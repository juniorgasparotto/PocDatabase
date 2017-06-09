<a href="https://github.com/juniorgasparotto/PocDatabase" rel="Inglês">
  <img alt="Inglês" src="https://github.com/juniorgasparotto/PocDatabase/blob/master/doc/img/en-us.png" />
</a>

<a href="https://github.com/juniorgasparotto/PocDatabase/blob/master/readme-pt-br.md" rel="Português">
  <img alt="Português" src="https://github.com/juniorgasparotto/PocDatabase/blob/master/doc/img/pt-br.png" />
</a>

# PocDatabase

Esse projeto é útil como um banco de dados em forma de arquivos e exclusivo para POCs ou projetos que estão em fase de criação e ainda não tem a estrutura de classes bem definida. 

O objetivo principal é a velocidade no desenvolvimento, evitando o desperdicio de alterar suas tabelas de banco de dados toda vez que uma classe for alterada.

Recomendamos utiliza-lo até o amadurecimento das classes e após essa fase, migrar para um banco de dados relacional ou NoSQL.

```
Install-Package PocDatabase
```

# Como funciona?

Ele utiliza o pacote `Newtonsoft.Json` para fazer o trabalho de serialização e deserialização e para que ele conheça o conteúdo do arquivo é necessário a criação de uma classe que represente o esquema de suas classes principais.

As classes principais (que fazem parte do esquema) precisam, obrigatoriamente, ter um campo com o nome `Id` e que seja do tipo `Guid`. Isso é necessário para que os métodos `GetById`, `Update` e `Delete` funcionem. Outro ponto importante é que o método `Insert` irá popular esse campo quando ele estiver `null`.

**Exemplo**

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