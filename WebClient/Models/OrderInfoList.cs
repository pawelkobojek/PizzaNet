using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;

namespace WebClient.Models
{
    public class OrderInfoList
    {
        public List<OrderInfoDTO> Data { get; set; }
    }
}