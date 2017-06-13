[
  ![Inglês](https://github.com/juniorgasparotto/PocDatabase/blob/master/doc/img/en-us.png)
](https://github.com/juniorgasparotto/PocDatabase)
[
  ![Português](https://github.com/juniorgasparotto/PocDatabase/blob/master/doc/img/pt-br.png)
](https://github.com/juniorgasparotto/PocDatabase/blob/master/readme-pt-br.md)

# PocDatabase

This project is useful as a database in the form of files. It's exclusive to `POCs` or projects that are in the process of creation and still has the class structure.

The main objective is to speed development, avoiding the waste of change your database tables every time a class is changed.

We recommend to use it until the maturation of classes and after this phase, migrate to a relational database or NoSQL.

# Installation

```
Install-Package PocDatabase
```

# Release notes

https://github.com/juniorgasparotto/PocDatabase/releases

# How does it work?

It uses the `Newtonsoft.Json` package to do the work of serializing and deserializing and for that he knows the contents of the file it is necessary to create a class that represents the schema of your main classes.

The main classes (which are part of the schema) must necessarily have a field with the name `Id` and `Guid` type. This is necessary so that the methods `GetById` , `Update` and `Delete` work. Another important point is that the method `Insert` will populate this field when it is `null` .

**Example**

```csharp
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Schema
{
    public List<Customer> Customers { get; set; }
    public List<User> Users { get; set; }
}

public static Main(string[] args) {
    var pocFile = new PocFile<Schema>();
    var repository = new PocRepository<Schema, Customer>(pocFile);

    // insert a new customer
    var customer = new Customer
    customer.Name =  "John Doe";
    repository.Insert(customer);
    pocFile.Save();

    // find by id and update
    var customer2 = repository.GetById(customer.Id);
    customer2.Name = "New Name";
    repository.Update(customer2);
    pocFile.Save();

    // get all
    var all = repository.GetAll();

    // get filtered
    var filtered = repository.Get(f=>f.Name == "New Name");
    repository.Delete(filtered.First());
    pocFile.Save();

    // clean file
    pocFile.Truncate();

    // OR delete file
    pocFile.Drop();
}
```

* * *

<sub>This text was translated by a machine</sub>

https://github.com/juniorgasparotto/MarkdownGenerator