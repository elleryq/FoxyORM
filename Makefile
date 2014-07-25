DMCS=dmcs

all: foxpro.exe

NotORM.dll: NotORM/DbContext.cs NotORM/DbExpandoObject.cs
	$(DMCS) -t:library -out:$@ $^ -reference:System.Data.dll	

foxpro.exe: NotORM.dll foxpro.cs
	$(DMCS) -t:exe -out:$@ foxpro.cs -reference:NotORM.dll,System.Data.dll
