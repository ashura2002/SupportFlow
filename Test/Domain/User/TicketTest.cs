using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;
using Xunit;

namespace Domain.UnitTests.Entities;

public class TicketTests
{
    // ASSUMPTION: adjust to a value that exists in your Priority enum.
    private const Priority DefaultPriority = Priority.Low;

    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly Guid RequesterId = Guid.NewGuid();
    private static readonly Guid AgentId = Guid.NewGuid();

    private static Ticket CreateTicket() =>
        Ticket.Create("TCK-0001", "Login issue", "Cannot login to the portal", DefaultPriority, CategoryId, RequesterId);

    /// <summary>
    /// Builds a ticket in the desired status using ONLY public behavior (no reflection),
    /// so the tests also prove the happy-path lifecycle works.
    /// Open -> Assigned -> InProgress -> (WaitingForUser | Resolved -> Closed -> Reopened)
    /// </summary>
    private static Ticket CreateTicketIn(TicketStatus status)
    {
        var ticket = CreateTicket();
        if (status == TicketStatus.Open) return ticket;

        ticket.AssignAgent(AgentId);
        if (status == TicketStatus.Assigned) return ticket;

        ticket.StartProgress();
        if (status == TicketStatus.InProgress) return ticket;

        if (status == TicketStatus.WaitingForUser)
        {
            ticket.WaitForUser();
            return ticket;
        }

        ticket.Resolve();
        if (status == TicketStatus.Resolved) return ticket;

        ticket.Close();
        if (status == TicketStatus.Closed) return ticket;

        ticket.Reopen();
        if (status == TicketStatus.Reopened) return ticket;

        throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported status for test setup.");
    }

    public static IEnumerable<object[]> AllStatusesExcept(params TicketStatus[] allowed) =>
        Enum.GetValues<TicketStatus>()
            .Where(s => !allowed.Contains(s))
            .Select(s => new object[] { s });

    public static IEnumerable<object[]> NotOpen() => AllStatusesExcept(TicketStatus.Open);
    public static IEnumerable<object[]> NotAssigned() => AllStatusesExcept(TicketStatus.Assigned);
    public static IEnumerable<object[]> NotInProgress() => AllStatusesExcept(TicketStatus.InProgress);
    public static IEnumerable<object[]> NotWaitingForUser() => AllStatusesExcept(TicketStatus.WaitingForUser);
    public static IEnumerable<object[]> NotResolved() => AllStatusesExcept(TicketStatus.Resolved);
    public static IEnumerable<object[]> NotClosed() => AllStatusesExcept(TicketStatus.Closed);
    public static IEnumerable<object[]> NotReopened() => AllStatusesExcept(TicketStatus.Reopened);

    // ---------- Create ----------

    [Fact]
    public void Create_WithValidData_ReturnsOpenUnassignedTicket()
    {
        var ticket = Ticket.Create("TCK-0001", "Login issue", "Cannot login", DefaultPriority, CategoryId, RequesterId);

        Assert.Equal("TCK-0001", ticket.TicketNumber);
        Assert.Equal("Login issue", ticket.Title);
        Assert.Equal("Cannot login", ticket.Description);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(DefaultPriority, ticket.Priority);
        Assert.Equal(CategoryId, ticket.CategoryId);
        Assert.Equal(RequesterId, ticket.RequesterId);
        Assert.Null(ticket.AssignedAgentId);
        Assert.Null(ticket.DueAt);
        Assert.Null(ticket.DeletedAt);
    }

    [Fact]
    public void Create_TrimsTextFields()
    {
        var ticket = Ticket.Create("  TCK-0001  ", "  Login issue  ", "  Cannot login  ", DefaultPriority, CategoryId, RequesterId);

        Assert.Equal("TCK-0001", ticket.TicketNumber);
        Assert.Equal("Login issue", ticket.Title);
        Assert.Equal("Cannot login", ticket.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData("  ab  ")] // too short after trimming
    public void Create_WithInvalidTicketNumber_Throws(string? value)
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Ticket.Create(value!, "Login issue", "Cannot login", DefaultPriority, CategoryId, RequesterId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void Create_WithInvalidTitle_Throws(string? value)
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Ticket.Create("TCK-0001", value!, "Cannot login", DefaultPriority, CategoryId, RequesterId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void Create_WithInvalidDescription_Throws(string? value)
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Ticket.Create("TCK-0001", "Login issue", value!, DefaultPriority, CategoryId, RequesterId));
    }

    [Fact]
    public void Create_WithTextOfExactlyThreeCharacters_Succeeds()
    {
        var ticket = Ticket.Create("TCK", "abc", "xyz", DefaultPriority, CategoryId, RequesterId);

        Assert.Equal("abc", ticket.Title);
    }

    // ---------- AssignAgent ----------

    [Fact]
    public void AssignAgent_OnOpenTicket_AssignsAgentAndMovesToAssigned()
    {
        var ticket = CreateTicket();

        ticket.AssignAgent(AgentId);

        Assert.Equal(AgentId, ticket.AssignedAgentId);
        Assert.Equal(TicketStatus.Assigned, ticket.Status);
    }

    [Fact]
    public void AssignAgent_RaisesSupportAgentAssignedDomainEvent()
    {
        var ticket = CreateTicket();

        ticket.AssignAgent(AgentId);

        // ASSUMPTION: your Aggregate exposes raised events as `DomainEvents`. Rename if different.
        Assert.Single(ticket.DomainEvents.OfType<SupportAgentAssignedDomainEvent>());
    }

    [Fact]
    public void AssignAgent_WithEmptyGuid_Throws()
    {
        var ticket = CreateTicket();

        Assert.Throws<DomainRuleViolationException>(() => ticket.AssignAgent(Guid.Empty));
        Assert.Null(ticket.AssignedAgentId);
        Assert.Equal(TicketStatus.Open, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotOpen))]
    public void AssignAgent_WhenNotOpen_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.AssignAgent(Guid.NewGuid()));
    }

    [Fact]
    public void AssignAgent_WhenNotAssignedAgain_DoesNotRaiseEventOnFailure()
    {
        var ticket = CreateTicket();
        ticket.AssignAgent(AgentId);

        Assert.Throws<DomainRuleViolationException>(() => ticket.AssignAgent(Guid.NewGuid()));

        Assert.Single(ticket.DomainEvents.OfType<SupportAgentAssignedDomainEvent>());
        Assert.Equal(AgentId, ticket.AssignedAgentId);
    }

    // ---------- StartProgress ----------

    [Fact]
    public void StartProgress_WhenAssigned_MovesToInProgress()
    {
        var ticket = CreateTicketIn(TicketStatus.Assigned);

        ticket.StartProgress();

        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotAssigned))]
    public void StartProgress_WhenNotAssigned_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.StartProgress());
    }

    // ---------- WaitForUser / ResumeProgress ----------

    [Fact]
    public void WaitForUser_WhenInProgress_MovesToWaitingForUser()
    {
        var ticket = CreateTicketIn(TicketStatus.InProgress);

        ticket.WaitForUser();

        Assert.Equal(TicketStatus.WaitingForUser, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotInProgress))]
    public void WaitForUser_WhenNotInProgress_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.WaitForUser());
    }

    [Fact]
    public void ResumeProgress_WhenWaitingForUser_MovesToInProgress()
    {
        var ticket = CreateTicketIn(TicketStatus.WaitingForUser);

        ticket.ResumeProgress();

        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotWaitingForUser))]
    public void ResumeProgress_WhenNotWaitingForUser_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.ResumeProgress());
    }

    // ---------- Resolve ----------

    [Fact]
    public void Resolve_WhenInProgress_MovesToResolved()
    {
        var ticket = CreateTicketIn(TicketStatus.InProgress);

        ticket.Resolve();

        Assert.Equal(TicketStatus.Resolved, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotInProgress))]
    public void Resolve_WhenNotInProgress_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.Resolve());
    }

    // ---------- Close ----------

    [Fact]
    public void Close_WhenResolved_MovesToClosed()
    {
        var ticket = CreateTicketIn(TicketStatus.Resolved);

        ticket.Close();

        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotResolved))]
    public void Close_WhenNotResolved_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.Close());
    }

    // ---------- Reopen / StartProgressAfterReopen ----------

    [Fact]
    public void Reopen_WhenClosed_MovesToReopened()
    {
        var ticket = CreateTicketIn(TicketStatus.Closed);

        ticket.Reopen();

        Assert.Equal(TicketStatus.Reopened, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotClosed))]
    public void Reopen_WhenNotClosed_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.Reopen());
    }

    [Fact]
    public void StartProgressAfterReopen_WhenReopened_MovesToInProgress()
    {
        var ticket = CreateTicketIn(TicketStatus.Reopened);

        ticket.StartProgressAfterReopen();

        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Theory]
    [MemberData(nameof(NotReopened))]
    public void StartProgressAfterReopen_WhenNotReopened_Throws(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        Assert.Throws<DomainRuleViolationException>(() => ticket.StartProgressAfterReopen());
    }

    // ---------- Full lifecycle ----------

    [Fact]
    public void FullLifecycle_FromOpenToReopened_FollowsAllowedTransitions()
    {
        var ticket = CreateTicket();

        ticket.AssignAgent(AgentId);
        ticket.StartProgress();
        ticket.WaitForUser();
        ticket.ResumeProgress();
        ticket.Resolve();
        ticket.Close();
        ticket.Reopen();
        ticket.StartProgressAfterReopen();

        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    // ---------- UpdateTitle ----------

    [Fact]
    public void UpdateTitle_WithValidTitle_UpdatesAndTrims()
    {
        var ticket = CreateTicket();

        ticket.UpdateTitle("  New title  ");

        Assert.Equal("New title", ticket.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void UpdateTitle_WithInvalidTitle_ThrowsAndKeepsOldValue(string value)
    {
        var ticket = CreateTicket();

        Assert.Throws<DomainRuleViolationException>(() => ticket.UpdateTitle(value));
        Assert.Equal("Login issue", ticket.Title);
    }

    [Fact]
    public void UpdateTitle_WhenClosed_Throws()
    {
        var ticket = CreateTicketIn(TicketStatus.Closed);

        Assert.Throws<DomainRuleViolationException>(() => ticket.UpdateTitle("New title"));
    }

    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.Assigned)]
    [InlineData(TicketStatus.InProgress)]
    [InlineData(TicketStatus.WaitingForUser)]
    [InlineData(TicketStatus.Resolved)]
    [InlineData(TicketStatus.Reopened)]
    public void UpdateTitle_WhenNotClosed_Succeeds(TicketStatus status)
    {
        var ticket = CreateTicketIn(status);

        ticket.UpdateTitle("New title");

        Assert.Equal("New title", ticket.Title);
    }

    // ---------- UpdateDescription ----------

    [Fact]
    public void UpdateDescription_WithValidDescription_UpdatesAndTrims()
    {
        var ticket = CreateTicket();

        ticket.UpdateDescription("  New description  ");

        Assert.Equal("New description", ticket.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void UpdateDescription_WithInvalidDescription_ThrowsAndKeepsOldValue(string value)
    {
        var ticket = CreateTicket();

        Assert.Throws<DomainRuleViolationException>(() => ticket.UpdateDescription(value));
        Assert.Equal("Cannot login to the portal", ticket.Description);
    }

    [Fact]
    public void UpdateDescription_WhenClosed_Throws()
    {
        var ticket = CreateTicketIn(TicketStatus.Closed);

        Assert.Throws<DomainRuleViolationException>(() => ticket.UpdateDescription("New description"));
    }

    // ---------- SoftDelete ----------

    [Fact]
    public void SoftDelete_SetsDeletedAtToUtcNow()
    {
        var ticket = CreateTicket();
        var before = DateTime.UtcNow;

        ticket.SoftDelete();

        Assert.NotNull(ticket.DeletedAt);
        Assert.InRange(ticket.DeletedAt!.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public void SoftDelete_WhenAlreadyDeleted_IsIdempotent()
    {
        var ticket = CreateTicket();
        ticket.SoftDelete();
        var firstDeletedAt = ticket.DeletedAt;

        ticket.SoftDelete();

        Assert.Equal(firstDeletedAt, ticket.DeletedAt);
    }
}