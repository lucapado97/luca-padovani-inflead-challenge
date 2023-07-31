## Introduction

This document presents two different exercises, each of them useful to evaluate a specific set of proficencies: 
1. A study case, that focuses on design praxis and wants to emulate a (semi)realistic project to be analyzed and understood;
2. A coding exercise, that allows to test the proficency to good code design and readability and the knowledge and comprehension of formal conventions.

N.B. There is not right answer to the proposed questions, only personal and (hopefully) diverse approaches.
The will to understand the context and to critically elaborate the specifications will be rewarded.

## Study case: Marco Polo Heavy Industries

Marco Polo Heavy Industries (MPHI) manages a complex storage and shipping system of various building materials (DATA), sent from different mining sites spread around our solar system (SITE), gathered and refined to be sold.
At the same time MPHI daily collects orders (concerning DATA) from the clients (USERS) that can be provided by contacting one or more SITEs.
Every involved extraction site expose a web API REST interface to access local mining data.
DATA related informations change periodically based on local environment and work conditions (TREND) that should be assessed, though it is critical for USERS to be able to evaluate and choose the best compromise between cost and quality to fulfill their specific needs.

You want to design a web portal where USERS can show and analyze MPHI data and eventually place orders.

Features:
1. User registration;
2. Manage personal orders;
3. Provide monitoring and analysis to various SITE's TRENDs;
4. Order placement interface.

These are the extraction sites:
- Mars (MA)
- Venus (VE)
- Asteroids 1 (AST1)
- Asteroids 2 (AST2)
- Asteroids 3 (AST3)

The materials on sale:
- Dauterium (Dt) 
- Tritium (Tz)
- Iron (Ir)
- Nickel (Nk)

Availability:
- Tz > AST1, AST2
- Dt > MA, AST1, AST3
- Ir > MA, VE, AST3
- Nk > MA, VE

Please note: Each site owns a special database (SITE DATA) to store materials' registry with it's own structure (AST1, AST2, AST3 share the same).

### Questions

- Can you design this solution with scalability in mind? Give an example of high-level design, complete with project hyerarchy and mutual dependencies focusing mainly on the requirement "should be easy to handle new sites joining the system".
	view diagram

- The application uses different external services (some of them act with SITEs, others with centralized processes). How whould you link them to the structure?
	If these services are developed by external corporations I would create a 'middleman service' that talks to the external services and linked with the DB at the same level of MPHI service

- We want to trace all user requests to database using a personal token (received as header from HTTP request). How whould you manage to serve this token through out the solution (avoiding to pass it as parameter of each layer)?
	One solution is to log each http request at the highest level possible, another one is to create a DB user for each USER and then on each HTTP request create a DB context-connection using the said user, now the DBMS can take charge of the logging part  

- Sometimes the database structure needs to be updated. Do you know any strategy to make this updates formal and safe? (e.g. O/RM migration)
	I frequently use the Reverse-POCO-Generator. Given a DB connection it generates db context (Entity framework) and POCO classes which makes adding new entities or fields painless.

- Would you use one or more database technology to handle all persistence cases? Like what?
	The main goal of a Database is to achieve data persistence, to make it more robust we can duplicate data at application level (duplicating data on 2 or more Dbs) or at physical level (raid, backups etc)

- Internally, MA groups Ir and Nk as "metals". A metal has two special traits: Malleability, Ductility. How whould you represent this fact designing the related database entities?
	Since it's just two small fields i would collapse the ER upwards, this results in having only one Entity (Material) with two fields used only by Metals.

- The database structure is SITE specific, although some entities can be shared among them. How would you choose to handle this entities?
	The MPHI service reads data from the SITEs and maps it into MPHI entities used by the site. This way each site can have it's data representation which can be the mapped into MPHI one. 

- Are you proficient in DevOps activities?
	Not really, i have some experience with Docker, yaml and so on but not much.

- MPHI ecosystem is made up with many different applications, each one with its own cloud virtual server. We need to handle their life cicle in an automated fashion. What's your approach?
	Gitlab, with CI-CD pipelines the developer can tag a commit to make the project commited and others to be automatically built, tested, and deployed.

- Have you ever used Unit Test tools? What's your opinion about test-driven development?
	Yes i do. However i've never started a test-driven project, the tests always came after the code in my experience.

- How would you plan your Unit Test activities across the development processes? 
	I usually build test cases around libraries i make since they are shared to more projects and integration tests are mandatory.
## Coding Exercise

You want to create a dotnet Core solution that allow, through a WebApi interface (MYAPI), to search users from a external Api (EXAPI) endpoint (payed per-call).
- You can get data from [random-data](https://random-data-api.com/api/users/random_user?size=10);
- You want to store the results in a local database (to serve from persistence if user exists);
- Expose a search endpoint (that accept three filter strings: Gender, Email, Username)

N.B. You want to minimize the number of calls to EXAPI.

The solution should be designed with scalability and maintainability in mind (eventually ready to new db entities).
Database entities can reflect EXAPI data structure (although doesn't have to) but is important to expose data from MYAPI in the following form: 

```

public class UserDTO
{
	[Key]
	public string Id { get; set; }					// map from Uid
	public string Email { get; set; }
	public string Username { get; set; }
	public string FullName { get; set; }			// name + surname
	public string ProfilePicUrl { get; set; }
	public string Gender { get; set; }
	public string PhoneNumber { get; set; }
	public string Employment { get; set; }
	public string KeySkill { get; set; }
	public string AddressId { get; set; }

	public DateTime CreationDate { get; set; } 		//DB entry date
}

public class AddressDTO
{
	[Key]
	public string Id { get; set; }					// autogenerated
	public string City { get; set; }
	public string Street { get; set; }				// name + address
	public string ZipCode { get; set; }
	public string State { get; set; }

	public DateTime CreationDate { get; set; } 		//DB entry date
}

```
