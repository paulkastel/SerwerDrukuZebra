using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SerwerDrukuZebra
{
	[XmlRoot("Records")]
	public class RecordList : List<Record>
	{
	}
}
