using System;
using LiteDB;

namespace WebApi.Models
{
    public class DateInfo
    {
        [BsonId]
        public int DateInfoId { get; set; }

        [BsonIndex]
        public DateTime Date { get; set; }
    }
}
