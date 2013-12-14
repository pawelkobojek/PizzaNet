using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetDataModel.Model;

namespace PizzaService.Assemblers
{
    public class UserAssembler
    {
        public UserDTO ToSimpleDto(User user)
        {
            return new UserDTO
            {
                Email = user.Email,
                Name = user.Name,
                Address = user.Address,
                Phone = user.Phone,
                Rights = user.Rights,
                UserID = user.UserID,
                Password = user.Password
            };
        }

        internal void UpdateEntity(User user, UserDTO userDto)
        {
            user.Address = userDto.Address;
            user.Email = userDto.Email;
            user.Name = userDto.Name;
            user.Phone = userDto.Phone;
            user.Rights = userDto.Rights;
            //TODO przedyskutować
            //user.Password = userDto.Password;
        }

        public User ToUser(RegisterUserDTO user)
        {
            return new User
            {
                Email = user.Email,
                Name = user.Name,
                Address = user.Address,
                Phone = user.Phone,
                Rights = 1,
                Password = user.Password
            };
        }
    }
}