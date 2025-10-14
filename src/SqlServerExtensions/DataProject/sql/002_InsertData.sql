if not exists (select 1 from Department)
begin
	Insert into Department (
		Name
	) values (
		'IT'
	);

	Insert into Department (
		Name
	) values (
		'Security'
	);
end

GO

if not exists (select 1 from Employee)
begin
	

Insert into Employee (
	Name, IDepartment
) values (
	'Ignat Andrei', 1
);

Insert into Employee (
	Name, IDepartment
) values (
	'John Doe', 2
);

end
GO

