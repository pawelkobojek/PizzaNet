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

        internal void UpdateEntityUserLevel(User user, UserDTO userDto)
        {
            if (userDto.Address != null)
                user.Address = userDto.Address;
            if (userDto.Name != null)
                user.Name = userDto.Name;
            if (userDto.Password != null)
                user.Password = userDto.Password;
            if (userDto.Phone >= 0)
                user.Phone = userDto.Phone;
        }

        public User ToUser(UserDTO user)
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