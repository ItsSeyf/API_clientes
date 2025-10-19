create database Curso_DB;

use Curso_DB;

create table Clientes(
id int identity(1,1),
nombre varchar(100) not null,
edad int not null
Constraint PK_clientes primary key(id)
);

create table Usuarios(
id int identity(1,1),
username varchar(100) not null,
pass varchar(max) not null
Constraint PK_clientes primary key(id)
);