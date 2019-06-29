module FSharp.MongoDBTests

open NUnit.Framework
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Bson.Serialization

type RecTest = 
        {
            Id: string
            Foo: int
            Bar: string
        }

type UnionTest =
    | TestCase1 of int
    | TestCase2 of string
    | TestCase3 of float
    | TestCase4 of decimal
    | TestCase5 of bool
    | TestCase6 of RecTest
    | TestCase7 of RecTest option
    | TestCase8

type RecTestWithUnion = 
    {
        Id: BsonObjectId
        Foo: int
        Bar1: UnionTest
        Bar2: UnionTest
        Bar3: UnionTest
        Bar4: UnionTest
        Bar5: UnionTest
        Bar6: UnionTest
        Bar7: UnionTest
        Bar8: UnionTest
        Bar9: int list
    }

type SmallTest =    
    {
        Foo: UnionTest
    }

[<AutoOpen>]
module Helpers =
    let NewId() = 
        BsonObjectId(ObjectId.GenerateNewId())

module UnitTest = 
    let connectionString = "mongodb://localhost"
    let client = new MongoDB.Driver.MongoClient(connectionString)
    let database = client.GetDatabase("TestFSharpMongo")
    do 
        FSharp.MongoDB.SerializationProviderModule.Register()
        FSharp.MongoDB.Conventions.ConventionsModule.Register()
    

    [<Test>]
    let TestMethod1 () =
        let wildcard = FilterDefinition<RecTest>.op_Implicit("{}")
        let testCase = 
            {
                RecTest.Id = "Hi"
                RecTest.Foo = 5
                RecTest.Bar = "Baz"
            }

        let collection = database.GetCollection<RecTest>("RecTest")
        collection.DeleteManyAsync(wildcard) |> ignore
        collection.InsertOneAsync(testCase).Wait() |> ignore
        let saved = collection.Find(wildcard).FirstAsync().Result
        Assert.AreEqual(testCase,saved)

    [<Test>]
    let TestMethod2 () =
        let wildcard = FilterDefinition<RecTestWithUnion>.op_Implicit("{}") 
        let testCase = 
            {
                RecTestWithUnion.Id = NewId()
                RecTestWithUnion.Foo = 0
                RecTestWithUnion.Bar1 = TestCase1(2)
                RecTestWithUnion.Bar2 = TestCase2("BazBar")
                RecTestWithUnion.Bar3 = TestCase3(1.4)
                RecTestWithUnion.Bar4 = TestCase4(3.14M)
                RecTestWithUnion.Bar5 = TestCase5(true)
                RecTestWithUnion.Bar6 = TestCase6({ RecTest.Id = "Moo"; RecTest.Foo = 5; RecTest.Bar = "Baz"})
                RecTestWithUnion.Bar7 = TestCase7(None)
                RecTestWithUnion.Bar8 = TestCase8 
                RecTestWithUnion.Bar9 = [1; 2; 3; 4; 5]
            }

        let collection = database.GetCollection<RecTestWithUnion>("RecTestWithUnion")
        collection.DeleteManyAsync(wildcard).Wait() |> ignore
        collection.InsertOneAsync(testCase).Wait() |> ignore
        let saved = collection.Find(wildcard).FirstAsync().Result
        Assert.AreEqual(testCase,saved)

    [<Test>]
    let TestMethod3 () = 
        let wildcard = FilterDefinition<SmallTest>.op_Implicit("{}") 
        let testCase = 
            {
                SmallTest.Foo = UnionTest.TestCase7 None
            }
       
        let collection = database.GetCollection<SmallTest>("SmallTest")
        collection.DeleteManyAsync(wildcard).Wait() |> ignore
        collection.InsertOneAsync(testCase).Wait() |> ignore
        let saved = collection.Find(wildcard).FirstAsync().Result
        Assert.AreEqual(testCase,saved)

    [<Test>]
    let TestMethod4 () =
        let collection = database.GetCollection<RecTest>("RecTest")
        let serializer = BsonSerializer.LookupSerializer(typeof<RecTest>) :?> IBsonSerializer<RecTest>

        let index = Builders<RecTest>.IndexKeys.Ascending(propertyEx <@ fun (ev: RecTest) -> ev.Id @>)
        let _ = index.Render(serializer, collection.Settings.SerializerRegistry)
        ()