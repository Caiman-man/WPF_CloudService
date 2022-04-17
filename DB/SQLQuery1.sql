CREATE TABLE Users
(
	[user_id]		  [int]			  IDENTITY primary key,
	[login]			  [varchar] (60)  NULL,
	[password]		  [varchar] (60)  NULL,
	[folder]		  [varchar] (60)  NULL
)

select * from users

insert into users(login, password, folder)
values 
('Nolan Greyson', '1a', 'C:\\IvanovCloud\\Nolan Greyson')

insert into users(login, password, folder)
values 
('Mark Greyson', '2a', 'C:\\IvanovCloud\\Mark Greyson')

insert into users(login, password, folder)
values 
('3', '3', '"C:\\IvanovCloud\\test folder"')

drop table Users