DMCS=dmcs

all: foxpro.exe

FoxyORM.dll: FoxyORM/DbContext.cs FoxyORM/DbExpandoObject.cs
	$(DMCS) -t:library -out:$@ $^ -reference:System.Data.dll	

foxpro.exe: FoxyORM.dll foxpro.cs
	$(DMCS) -t:exe -out:$@ foxpro.cs -reference:FoxyORM.dll,System.Data.dll
