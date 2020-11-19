using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using MongoDB.Bson;
using MongoDB.Driver;


namespace UmbTestEvents.Classes
{
    public class SubscribeToContentServiceSavingComposer :IUserComposer
    {
        public void Compose(Composition composition) {
            composition.Components().Append<SubscribeToContentServiceSavingComponent>();
        }
    }

    public class SubscribeToContentServiceSavingComponent : IComponent{

        private MongoClient client = new MongoClient("mongodb+srv://BrianUser:TheBrianUserPassword@cluster0.xh1tn.mongodb.net/BrianDatabaseTest?retryWrites=true&w=majority");

        public void Initialize() {
            ContentService.Saving += ContentService_Saving;
        }

        public void Terminate()
        {
            ContentService.Saving -= ContentService_Saving;
        }

        public void ContentService_Saving(IContentService sender, ContentSavingEventArgs e) {
            System.Diagnostics.Debug.WriteLine("Saving & publishing");
            foreach (var content in e.SavedEntities.Where(c => c.ContentType.Alias.InvariantEquals("MyContentType"))){
                System.Diagnostics.Debug.WriteLine("Content found");
                var umbContentValue = content.GetValue("rTEtest");
                System.Diagnostics.Debug.WriteLine(umbContentValue);

                var database = client.GetDatabase("BrianDatabaseTest");
                var collection = database.GetCollection<BsonDocument>("BrianCollectionTest");
                var document = new BsonDocument { { "theValue", umbContentValue.ToString() } };
                collection.InsertOne(document);
            }
        }
    }
}