﻿using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zendesk.JsonSearch.Logic;
using Zendesk.JsonSearch.Logic.CustomExceptions;
using Zendesk.JsonSearch.Models;

namespace Zendesk.JSonSearch.Logic.Test
{
    public class FrameworkTests
    {
        Entity userSample;
        Entity ticketSample;
        [SetUp]
        public void Setup()
        {
            Framework.Initialise();

            userSample = new Entity("1", "user");
            userSample.name = "Francisca Rasmussen";
            userSample.Attributes.Add(new KeyValuePair<string, object>("name", "Francisca Rasmussen"));
            userSample.Attributes.Add(new KeyValuePair<string, object>("created_at", "2016-04-15T05:19:46-10:00"));
            userSample.Attributes.Add(new KeyValuePair<string, object>("verified", true));
            userSample.Attributes.Add(new KeyValuePair<string, object>("ticket", new List<string> { "A Problem in Russian Federation", "A Problem in Malawi" }));
            

            ticketSample = new Entity("436bf9b0-1147-4c0a-8439-6f79833bff5b", "ticket");
            ticketSample.name = "A Catastrophe in Korea (North)";
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("created_at", "2016-04-28T11:19:34-10:00"));
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("type", "incident"));
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("subject", "A Catastrophe in Korea (North)"));
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("assignee_id", "24"));
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("assignee_name", "Harris Côpeland"));
            ticketSample.Attributes.Add(new KeyValuePair<string, object>("tags", new List<string> { "Ohio", "Pennsylvania", "American Samoa", "Northern Mariana Islands" }));
        }

        #region Test Wrong JSON
        [Test]
        public void TestWrongDirectory()
        {
            Assert.Catch<DirectoryNotFoundException>(() => Framework.Initialise("appsettings_wrongdirectory.json"));
        }


        [Test]
        public void TestWrongJSONFormat()
        {
            Assert.Catch<JsonDeseriliseException>(() => Framework.Initialise("appsettings_wrongjsonformat.json"));
        }

        [Test]
        public void TestWrongFileName()
        {
            Assert.Catch<FileNotFoundException>(() => Framework.Initialise("appsettings_wrongfilename.json"));
        }
        #endregion

        #region Test Memory Data
        [Test]
        public void TestMetadataCount()
        {
            Assert.IsTrue(Framework.Metadata.Entities.Count == 2);
            Assert.IsTrue(Framework.Metadata.Relationships.Count == 1);
        }

        [Test]
        public void TestCachedEntityCount()
        {
            Assert.IsTrue(Framework.CahcedEntityCollection.EntityCollection.Count == 2);
            Assert.IsTrue(Framework.CahcedEntityCollection.EntityCollection["users"].Entities.Count == 75);
            Assert.IsTrue(Framework.CahcedEntityCollection.EntityCollection["tickets"].Entities.Count == 200);
        }

        [Test]
        public void TestCachedIndexingCount()
        {
            Assert.IsTrue(Framework.CahcedIndexingCollection.IndexingCollection.Count == 2);
            Assert.IsTrue(Framework.CahcedIndexingCollection.IndexingCollection["users"].Count == 2);
            Assert.IsTrue(Framework.CahcedIndexingCollection.IndexingCollection["tickets"].Count == 4);
            Assert.IsTrue(Framework.CahcedIndexingCollection.IndexingCollection["users"]["verified"].Indexings.Count == 2);
            Assert.IsTrue(Framework.CahcedIndexingCollection.IndexingCollection["tickets"]["type"].Indexings.Count == 4);
        }
        #endregion

        #region Happy pass - result expected
        [Test]
        public void TestSearchWithIdAndCorrectValueWithRelatedToEntities()
        {
            var users = Framework.Search("users", "_id", 1);
            Assert.IsTrue(users.Count == 1);
            var user = users.First();
            Assert.IsTrue(IsUserSame(user,userSample));
        }

        [Test]
        public void TestSearchWithIdAndCorrectValueWithRelatedFromEntities()
        {
            var tickets = Framework.Search("tickets", "_id", "436bf9b0-1147-4c0a-8439-6f79833bff5b");
            Assert.IsTrue(tickets.Count == 1);
            var ticket = tickets.First();
            Assert.IsTrue(IsTicketSame(ticket, ticketSample));
        }

        [Test]
        public void TestSearchWithIndexingPropertyAndCorrectValue()
        {
            var users = Framework.Search("users", "name", "Francisca Rasmussen");
            Assert.IsTrue(users.Count == 1);
            var user = users.First();
            Assert.IsTrue(IsUserSame(user, userSample));

            var tickets = Framework.Search("tickets", "type", "incident");
            Assert.IsTrue(tickets.Count == 35);
            var ticket = tickets.First();
            Assert.IsTrue(IsTicketSame(ticket, ticketSample));
        }

        [Test]
        public void TestSearchWithNonIndexingPropertyAndCorrectValue()
        {
            var users = Framework.Search("users", "created_at", "2016-04-15T05:19:46-10:00");
            Assert.IsTrue(users.Count == 1);
            var user = users.First();
            Assert.IsTrue(IsUserSame(user, userSample));

            var tickets = Framework.Search("tickets", "subject", "A Catastrophe in Korea (North)");
            Assert.IsTrue(tickets.Count == 1);
            var ticket = tickets.First();
            Assert.IsTrue(IsTicketSame(ticket, ticketSample));
        }

        [Test]
        public void TestSearchWithListStringAndCorrectValue()
        {
            var tickets = Framework.Search("tickets", "tags", "Ohio");
            Assert.IsTrue(tickets.Count == 14);
            var ticket = tickets.First();
            Assert.IsTrue(IsTicketSame(ticket, ticketSample));
        }

        #endregion

        #region unhappy pass - expect null

        [Test]
        public void TestSearchWithIdAndWrongValue()
        {
            var users = Framework.Search("users", "_id", 999);
            Assert.IsTrue(users == null);
            var tickets = Framework.Search("tickets", "_id", 999);
            Assert.IsTrue(tickets == null);
        }



        [Test]
        public void TestSearchWithIndexingPropertyAndWrongValue()
        {
            var users = Framework.Search("users", "name", "wrongvalue");
            Assert.IsTrue(users == null);

            var tickets = Framework.Search("tickets", "type", "wrongvalue");
            Assert.IsTrue(tickets == null);
        }



        [Test]
        public void TestSearchWithNonIndexingPropertyAndWrongValue()
        {
            var users = Framework.Search("users", "created_at", "wrongvalue");
            Assert.IsTrue(users == null);

            var tickets = Framework.Search("tickets", "subject", "wrongvalue");
            Assert.IsTrue(tickets == null);
        }


        [Test]
        public void TestSearchWithListStringAndWrongValue()
        {
            var tickets = Framework.Search("tickets", "tags", "wrongvalue");
            Assert.IsTrue(tickets == null);
        }

        [Test]
        public void TestSearchWithWrongEntity()
        {
            var entities = Framework.Search("wrongentity", "_id", 999);
            Assert.IsTrue(entities == null);
        }

        [Test]
        public void TestSearchWithWrongProperty()
        {
            var entities = Framework.Search("users", "wrongproperty", 999);
            Assert.IsTrue(entities == null);
        }

        #endregion

        private bool IsUserSame(Entity entity, Entity user)
        {
            var isSame = entity._id == user._id
                && entity.entityName == user.entityName
                && entity.name == user.name
                && entity.GetAttribute<string>("created_at") == user.GetAttribute<string>("created_at")
                && entity.GetAttribute<bool>("verified") == user.GetAttribute<bool>("verified");
            var tickets = entity.GetAttribute<List<string>>("ticket");
            if (tickets.Any())
            {
                for (var i = 0; i < tickets.Count; i++)
                {
                    isSame = isSame && tickets[i] == user.GetAttribute<List<string>>("ticket")[i];
                }
            }
            return isSame;
        }

        private bool IsTicketSame(Entity entity, Entity ticket)
        {
            var isSame = entity._id == ticket._id
                && entity.entityName == ticket.entityName
                && entity.name == ticket.name
                && entity.GetAttribute<string>("created_at") == ticket.GetAttribute<string>("created_at")
                && entity.GetAttribute<string>("type") == ticket.GetAttribute<string>("type")
                && entity.GetAttribute<string>("assignee_id") == ticket.GetAttribute<string>("assignee_id")
                && entity.GetAttribute<string>("assignee_name") == ticket.GetAttribute<string>("assignee_name");
            var tags = entity.GetAttribute<List<string>>("tags");
            if (tags.Any())
            {
                for (var i = 0; i < tags.Count; i++)
                {
                    isSame = isSame && tags[i] == ticket.GetAttribute<List<string>>("tags")[i];
                }
            }
            return isSame;
        }
    }
}