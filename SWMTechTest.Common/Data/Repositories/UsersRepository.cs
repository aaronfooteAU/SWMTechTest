using Dapper;
using MySqlConnector;
using SWMTechTest.Common.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SWMTechTest.Common.Data.Repositories
{
    public interface IUsersRepository
    {
        public Task<User> GetUserById(int id);

        public Task<IEnumerable<User>> GetUsersByAge(int age);

        public Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize);

        public Task UpsertUser(User user);

        public Task DeleteUserById(int id);
    }

    [ExcludeFromCodeCoverage]
    public class UsersRepository : IUsersRepository
    {
        private readonly MySqlConnection _conn;

        public UsersRepository(MySqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<User> GetUserById(int id)
        {
            const string sql = @"
                select
                    id,
                    firstName,
                    lastName,
                    age,
                    gender
                from
                    users
                where
                    id = @id
            ";

            return await _conn.QueryFirstOrDefaultAsync<User>(sql, new { id }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetUsersByAge(int age)
        {
            const string sql = @"
                select
                    id,
                    firstName,
                    lastName,
                    age,
                    gender
                from
                    users
                where
                    age = @age
            ";

            return await _conn.QueryAsync<User>(sql, new { age }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new Exception($"pageNumber parameter must be >= 1 and pageSize parameter must be >= 1.  pageNumber: '{pageNumber}' && pageSize: '{pageSize}' was specified");

            var offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                select
                    id,
                    firstName,
                    lastName,
                    age,
                    gender
                from
                    users
                order by id
                limit @offset, @pageSize
            ";

            return await _conn.QueryAsync<User>(sql, new { offset, pageSize }).ConfigureAwait(false);
        }

        public async Task UpsertUser(User user)
        {
            const string sql = @"
                INSERT INTO users
                    (id, firstName, lastName, age, gender)
                VALUES
                    (@id, @firstName, @lastName, @age, @gender)
                ON DUPLICATE KEY UPDATE
                    firstName = @firstName,
                    lastName = @lastName,
                    age = @age,
                    gender = @gender;
                ";

            await _conn.ExecuteAsync(sql, new { id = user.Id, firstName = user.FirstName, lastName = user.LastName, age = user.Age, gender = user.Gender.ToString() }).ConfigureAwait(false);
        }

        public async Task DeleteUserById(int id)
        {
            const string sql = @"
                DELETE FROM users
                WHERE
                    id = @id
                ";

            await _conn.ExecuteAsync(sql, new { id }).ConfigureAwait(false);
        }
    }
}