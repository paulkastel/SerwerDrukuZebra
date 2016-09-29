using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SerwerDrukuZebra
{
	[XmlType("Record")]
	public struct Record
	{
		public string nazwaMagazynu
		{
			get;
			set;
		}
		public string nazwaBlachy
		{
			get;
			set;
		}
        public string typBlachy
		{
			get;
			set;
		}
        public string rodzajBlachy
		{
			get;
			set;
		}
		public double widthBlachy
		{
			get;
			set;
		}
		public double heightBlachy
		{
			get;
			set;
		}
		public int mkw
		{
			get;
			set;
		}
		public double masaton
		{
			get;
			set;
		}
		public string data
		{
			get;
			set;
		}
		public string kodKreskowy
		{
			get;
			set;
		}
	}
}
