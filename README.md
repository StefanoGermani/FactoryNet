PlantFarm
=====

PlantFarm is a test factory for .NET 3.5.  It is very much like FactoryGirl (http://github.com/thoughtbot/factory_girl/) for Ruby.  The goal of this project is to allow you to reduce noise and duplication across your tests when creating models.  

Download
--------

* Github: http://github.com/jbrechtel/plant
* NuGet:  https://nuget.org/packages/Plant (Thanks Chris Micacchi!)


Features
--------

Currently PlantFarm supports

* Object creation via properties
* Object creation by constructor arguments
* Overriding default property and constructor argument values
* Modular object definition
* Lazily evaluated property and constructor argument values
* Sequenced properties
* Allow user to specify after-create actions on models (to save the model to the DB after creation, for instance)

Targetted features can be found in the issues list for the project.  Some specific ones are

* Allowing multiple different definitions for one object
* Specify associated instances


Terms
-----

A 'Plant' is the thing that creates your objects for you.  
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
        plant.DefinePropertiesOf<Person>(new
                               {
                                  ID = Sequence.Evaluate((sequenceValue) => sequenceValue),
                                  Name = Sequence.Evaluate((sequenceValue) => string.Format("Name {0}", sequenceValue))
                               });
      }
    }
  
  
Usage
-----

To create a new Plant, you'll typically want to tell it which Assembly to look in for Blueprints.  You can do this via

    var plant = new BasePlant().WithBlueprintsFromAssemblyOf<PersonBlueprint>();
  
where PersonBlueprint is one of the Blueprints you have defined.  Plant will then load blueprints from any other type that implements the Blueprint interface in that assembly.

To retrieve the default instance of an object

    var person = plant.Create<Person>();
  
To retrieve an instance of a person with specific parts of the default blueprint overridden

    var person = plant.Create<Person>(p => p.EmailAddress = "john@doe.com" );
  
Multiple properties can be overridden in one call

    var person = plant.Create<Person>(p => { p.EmailAddress = "john@doe.com"; p.State = "GA"; });
