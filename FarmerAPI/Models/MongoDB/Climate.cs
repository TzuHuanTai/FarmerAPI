using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAPI.Models.MongoDB
{
    public class Climate
    {
		// auto generator _id
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public ObjectId Id { get; set; }

		[BsonElement("StationId")]
		[BsonRepresentation(BsonType.Int32)]
		public int StationId { get; set; }

		[BsonElement("ObsTime")]
		[BsonRepresentation(BsonType.DateTime)]
		[BsonIgnoreIfDefault]
		public DateTime? ObsTime { get; set; }

		[BsonElement("StnPres")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? StnPres { get; set; }

		[BsonElement("SeaPres")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? SeaPres { get; set; }

		[BsonElement("Temperature")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? Temperature { get; set; }

		[BsonElement("Td")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? Td { get; set; }

		[BsonElement("RH")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? RH { get; set; }

		[BsonElement("WS")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? WS { get; set; }

		[BsonElement("WD")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? WD { get; set; }

		[BsonElement("WSGust")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? WSGust { get; set; }

		[BsonElement("WDGust")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? WDGust { get; set; }

		[BsonElement("Precp")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? Precp { get; set; }

		[BsonElement("PrecpHour")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? PrecpHour { get; set; }

		[BsonElement("SunShine")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? SunShine { get; set; }

		[BsonElement("GlobalRad")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? GlobalRad { get; set; }

		[BsonElement("Visb")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? Visb { get; set; }

		[BsonElement("Lux")]
		[BsonRepresentation(BsonType.Decimal128)]
		[BsonIgnoreIfDefault]
		public decimal? Lux { get; set; }
	}
}
