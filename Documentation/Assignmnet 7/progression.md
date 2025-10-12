# Ass. 7, Completed Features Status

* [x] Read the ass. description
* [x] Prep the environment
* [x] Specify what need to be done here, step by step:
* [ ] MAYBE LATER?
  Configure EFC with https://troelsmortensen.github.io/CodeLabs/Tutorials/DddWithEfc/Page.html
* [ ] Implement the Repository pattern
  * [ ] generic base class
  * [ ] generic base interface
  * [ ] specific interfaces
  * [ ] implementing classes (utilize EFC)
  * [ ] implementing the Unit of Work (utilize EFC)
*
* [ ] **3) DbContext.** You may organize the directories in your persistence project as you like,
  but please put a little thought into organization. Rework this structure as needed.
  Create the custom DbContext, call it something relevant, DmContext, AppDbContext,
  EfcDbContext, WriteDbContext, DomainModelContext, or whatever.
* [ ]  **4) Configure the DbContext.** add relevant DbSets to your DbContext,
  and configure the mapping, so that your Domain Model is stored correctly.
* [ ] **5) Design Time Context Factory.** 
* [ ] **6) Optional: Integration tests.**
* [ ] **7) Configure the DbContext according to the guide linked above**
* [ ] **8) Implement Unit of Work**
* [ ] **9) Implement Repositories**
* [ ] **Challenge: Soft Delete feature.** 
 <br>Can you implement a Soft Delete for the Guest without modifying the Guest Aggregate? Or the Player.
 <br>You will need a shadow property. These are discussed in the configuration guide. They are properties only present in the database, not the Domain Model.
 <br>How do you access users (Guest/Player) with deleted users automatically filtered out?
