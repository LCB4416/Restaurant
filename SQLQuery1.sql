CREATE TABLE Menuitem(
 ID int NOT NULL Primary Key IDENTITY(1,1),
  MenuName varChar(25),
  Price money,
  Description varChar(250),
);

select * 
FROM Menuitem