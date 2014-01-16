using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetDataModelEntityInfrastructure.Model;

namespace PizzaService.Assemblers
{
    public class ComplaintAssembler
    {
        public ComplaintDTO ToSimpleDto(Complaint c)
        {
            return new ComplaintDTO
            {
                Body = c.Body,
                ComplaintID = c.ComplaintID
            };
        }

        public Complaint ToComplaint(ComplaintDTO c)
        {
            return new Complaint
            {
                ComplaintID = c.ComplaintID,
                Body = c.Body
            };
        }
    }
}