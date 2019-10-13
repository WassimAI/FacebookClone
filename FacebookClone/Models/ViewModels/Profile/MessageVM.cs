using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class MessageVM
    {
        public MessageVM()
        {

        }
        public MessageVM(MessageDTO row)
        {
            Id = row.Id;
            From = row.From;
            To = row.To;
            Message = row.Message;
            DateSent = row.DateSent;
            Read = row.Read;
            FromId = row.FromUser.Id;
            FromUsername = row.FromUser.Username;
            FromFirstName = row.FromUser.FirstName;
            FromLastName = row.FromUser.LastName;
        }
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }

        public int FromId { get; set; }
        public string FromUsername { get; set; }
        public string FromFirstName { get; set; }
        public string FromLastName { get; set; }
    }
}