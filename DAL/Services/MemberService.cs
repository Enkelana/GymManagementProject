using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.Data;
using GymManagementProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GymManagementProject.DAL.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext context;
        public MemberService(ApplicationDbContext context)
        {
            this.context = context;
        }
        private string GenerateRandomCardCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "1234567890";
            StringBuilder result = new StringBuilder(7);
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            result.Append("-");
            for (int i = 0; i < 3; i++)
            {
                result.Append(numbers[random.Next(numbers.Length)]);
            }
            return result.ToString();
        }
        public async Task<MemberDTO> CreateMember(MemberDTO memberDto)
        {
            if (memberDto.Birthday >= DateTime.Today)
            {
                throw new ArgumentException("Birthday cannot be today or a future date.");
            }
            try
            {
                var member = new Member
                {
                    Id = memberDto.Id,
                    FirstName = memberDto.FirstName,
                    LastName = memberDto.LastName,
                    Birthday = memberDto.Birthday,
                    IdCardNumber = memberDto.IdCardNumber,
                    Email = memberDto.Email,
                    RegistrationDate = DateTime.Today, 
                    IsDeleted = memberDto.IsDeleted,
                    RegistrationCard = GenerateRandomCardCode()
                };

                if (await context.Members.AnyAsync(m => m.IdCardNumber == member.IdCardNumber))
                    throw new ArgumentException("A member with this ID card number exists.");

                context.Members.Add(member);
                await context.SaveChangesAsync();

                var member_dto = new MemberDTO
                {
                    Id = member.Id,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Birthday = member.Birthday,
                    IdCardNumber = member.IdCardNumber,
                    Email = member.Email,
                    RegistrationDate = member.RegistrationDate, 
                    IsDeleted = member.IsDeleted,
                    RegistrationCard = member.RegistrationCard
                };

                return member_dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MemberDTO>> SearchMember(string searchTerm)
        {
            try
            {
                var lowerSearchTerm = searchTerm.ToLower();
                var members = await context.Members.ToListAsync();

                var filteredMembers = members
                    .Where(m => m.FirstName.ToLower().Contains(lowerSearchTerm) ||
                                m.LastName.ToLower().Contains(lowerSearchTerm) ||
                                m.IdCardNumber.ToLower().Contains(lowerSearchTerm) ||
                                m.Email.ToLower().Contains(lowerSearchTerm))
                    .ToList();

                return filteredMembers.Select(m => new MemberDTO
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Birthday = m.Birthday,
                    IdCardNumber = m.IdCardNumber,
                    Email = m.Email,
                    RegistrationDate = m.RegistrationDate.Date,
                    IsDeleted = m.IsDeleted,
                    RegistrationCard = m.RegistrationCard
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<MemberDTO> GetByIdMember(int id)
        {
            try
            {
                var member = await context.Members.FirstAsync(x => x.Id == id);

                if (member == null)
                { throw new ArgumentException("The specified member could not be found."); }

                return new MemberDTO
                {
                    Id = member.Id,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Birthday = member.Birthday,
                    IdCardNumber = member.IdCardNumber,
                    Email = member.Email,
                    RegistrationDate = member.RegistrationDate.Date,
                    IsDeleted = member.IsDeleted,
                    RegistrationCard = member.RegistrationCard
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<MemberEditDTO> UpdateMember(MemberEditDTO memberDto)
        {
            try
            {
                var existingMember = await context.Members.FirstOrDefaultAsync(x => x.Id == memberDto.Id);

                if (existingMember == null)
                {
                    throw new ArgumentException("The specified member could not be found.");
                }

                existingMember.Email = memberDto.Email;

                if (memberDto.IsDeleted)
                {
                    existingMember.IsDeleted = true;

                    var memberSubscriptions = await context.MemberSubscriptions
                        .Where(ms => ms.MemberId == existingMember.Id && ms.IsDeleted == false)
                        .ToListAsync();

                    foreach (var subscription in memberSubscriptions)
                    {
                        subscription.IsDeleted = true;
                    }

                    context.MemberSubscriptions.UpdateRange(memberSubscriptions);
                }
                else
                {
                    existingMember.IsDeleted = false;
                }

                context.Members.Update(existingMember);
                await context.SaveChangesAsync();

                return new MemberEditDTO
                {
                    Id = existingMember.Id,
                    Email = existingMember.Email,
                    IsDeleted = existingMember.IsDeleted
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<MemberDTO>> GetAllMembers()
        {
            try
            {
                var members = await context.Members
                    .ToListAsync();

                return members.Select(m => new MemberDTO
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Birthday = m.Birthday,
                    IdCardNumber = m.IdCardNumber,
                    Email = m.Email,
                    RegistrationDate = m.RegistrationDate,
                    IsDeleted = m.IsDeleted,
                    RegistrationCard = m.RegistrationCard
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<MemberDTO>> GetAllActiveMembers()
        {
            try
            {
                var members = await context.Members
                    .Where(m => !m.IsDeleted)
                    .ToListAsync();

                return members.Select(m => new MemberDTO
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Birthday = m.Birthday,
                    IdCardNumber = m.IdCardNumber,
                    Email = m.Email,
                    RegistrationDate = m.RegistrationDate,
                    IsDeleted = m.IsDeleted,
                    RegistrationCard = m.RegistrationCard
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
