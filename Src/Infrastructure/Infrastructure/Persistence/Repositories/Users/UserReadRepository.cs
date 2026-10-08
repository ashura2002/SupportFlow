﻿using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Users
{
    public sealed class UserReadRepository : IUserReadRepository
    {
        private readonly SupportFlowDbContext _context;
        public UserReadRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponse?> GetAdminAsync(CancellationToken ct)
        {
            return await _context.Database
                .SqlQuery<UserResponse>(
               $"""
                SELECT 
                    "Id",
                    "FirstName",
                    "LastName",
                    "Email",
                    "Role"
                FROM "Users"
                WHERE "Role" = {(int)Roles.Administrator} AND
                "DeletedAt" IS NULL
                """)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PaginatedResult<UserResponse>> GetAllActiveUsersAsync(int page, int pageSize, CancellationToken ct)
        {
            var totalCount = await _context.Database
                .SqlQuery<int>(
                $"""
                    SELECT COUNT(*) AS "Value"
                    FROM "Users"
                    WHERE "DeletedAt" IS NULL
                """)
                .SingleAsync(ct);

            var users = await _context.Database
                .SqlQuery<UserResponse>(
                $"""
                    SELECT 
                        "Id",
                        "FirstName",
                        "LastName",
                        "Role",
                        "Email"
                FROM "Users"
                WHERE "DeletedAt" IS NULL
                ORDER BY "FirstName", "LastName"
                OFFSET {(page - 1) * pageSize}
                LIMIT {pageSize}
                """)
                .ToListAsync(ct);

            return new PaginatedResult<UserResponse>(
                users,
                page,
                pageSize, 
                totalCount);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Database
                .SqlQuery<UserResponse>(
                $"""
                 SELECT 
                    "Id",
                    "FirstName",
                    "LastName",
                    "Email",
                    "Role"
                FROM "Users"
                WHERE "Id" = {userId} AND 
                "DeletedAt" IS NULL
                """)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsEmailExist(string email, CancellationToken ct)
        {
            var emailVo = Email.Create(email);
            return await _context.Database
                .SqlQuery<bool>(
               $"""
                SELECT EXISTS (
                    SELECT 1
                    FROM "Users"
                    WHERE "Email" = {emailVo.Value} AND 
                    "DeletedAt" IS NULL
                ) AS "Value"
                """)
                .SingleAsync(ct);
        }
    }
}