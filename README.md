FactoryNet
=====

FactoryNet is a test factory for .NET 3.5. It is very much like FactoryGirl (http://github.com/thoughtbot/factory_girl/) for Ruby. The goal of this project is to allow you to reduce noise and duplication across your tests when creating models.  

Download
--------

* Github: http://github.com/stefanogermani/factorynet


Features
--------

Currently FactoryNet supports

* Object creation via properties
* Object creation by constructor arguments
* Overriding default property and constructor argument values
* Modular object definition
* Lazily evaluated property and constructor argument values
* Sequenced properties
* Allow user to specify after-create actions on models (to save the model to the DB after creation, for instance)
* Allowing multiple different definitions for one object

Targetted features can be found in the issues list for the project.  Some specific ones are

* Specify associated instances


Terms
-----

A 'Factory' is the thing that creates your objects for you.  
A 'Blueprint' is what a user provides to tell a plant how to create an object.

Defining a Blueprint
--------------------

A Blueprint is just a class that implements the Blueprint interface.  The interface defines one method, SetupPlant, which takes a BasePlant object.  SetupPlant is a generic method which whose generic argument is the Type that you're setting up and an anonymous object with the appropriate properties.

Note that currently property validation occurs during object creation, not Define.

    class PersonBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.Define(() => new Person("Barbara")
                               {
                                  LastName = "Brechtel",
                                  Address = "111 South Main St.",
                                  City = "Gulfport",
                                  State = "MS",
                                  EmailAddress = "barbara@brechtel.com"s
                               });
      }
    }
  

Sequenced properties
---------------------------

To define a Blueprint property that is evaluated lazily, but with a sequence counter, set the value to Sequence.Evaluate(lambda) like so:

    class PersonBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.Define(() => new Person
                           {
                              ID = Sequence.Evaluate((i) => i),
                              Name = Sequence.Evaluate((i) => string.Format("Name {0}", i))
                           });
      }
    }
  
  
Usage
-----

To create a new Factory, you'll typically want to tell it which Assembly to look in for Blueprints.  You can do this via

    var factory = new BaseFactory().LoadBlueprintsFromCurrentAssembly();
  
where PersonBlueprint is one of the Blueprints you have defined.  Plant will then load blueprints from any other type that implements the Blueprint interface in that assembly.

To retrieve the default instance of an object

    var person = factory.Create<Person>();
  
To retrieve an instance of a person with specific parts of the default blueprint overridden

    var person = factory.Create<Person>(p => p.EmailAddress = "john@doe.com" );
  
Multiple properties can be overridden in one call

    var person = factory.Create<Person>(p => { p.EmailAddress = "john@doe.com"; p.State = "GA"; });
