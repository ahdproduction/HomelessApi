using System;

namespace HomelessApi.Models
{
	public class CustomPin : MongoDocument
    {
        public string Url { get; set; }
        public PinTypeEnum PinType { get; set; }
        public string ViewId { get; set; }
    }

	public enum PinTypeEnum
    {
        Homeless,
        Pet,
        User
    }
}
