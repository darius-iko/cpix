﻿using Axinom.Cpix;
using System.IO;
using System.Text;
using System.Xml;
using Tests;

namespace TestVectorGenerator
{
	sealed class Invalid_BadContentKeysSignature : ITestVector
	{
		public string Description => @"The signature on the content key collection should fail validation because one of the content key elements was removed after applying the signature.";
		public bool OutputIsValid => false;

		public void Generate(Stream outputStream)
		{
			var document = new CpixDocument();

			document.ContentKeys.Add(TestHelpers.GenerateContentKey());
			document.ContentKeys.Add(TestHelpers.GenerateContentKey());
			document.ContentKeys.AddSignature(TestHelpers.Certificate4WithPrivateKey);

			var buffer = new MemoryStream();
			document.Save(buffer);

			var xml = new XmlDocument();
			buffer.Position = 0;
			xml.Load(buffer);

			var namespaces = XmlHelpers.CreateCpixNamespaceManager(xml);

			var firstContentKey = (XmlElement)xml.SelectSingleNode("/cpix:CPIX/cpix:ContentKeyList/cpix:ContentKey", namespaces);
			firstContentKey.ParentNode.RemoveChild(firstContentKey);

			using (var writer = XmlWriter.Create(outputStream, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				CloseOutput = false
			}))
			{
				xml.Save(writer);
			}
		}
	}
}
