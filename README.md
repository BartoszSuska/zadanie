SPECYFIKACJA TECHNICZNA

Opis klas i metod:
Klasa: CategoriesController
Klasa odpowiada za zwracanie kategorii z bazy danych.
Metody: Get() - zwraca kategorię w formie listy.

Klasa: SubcategoriesController
Klasa odpowiada za zwracanie podkategorii z bazy danych.
Metody: Get() - zwraca podkategorię w formie listy.

Klasa: ContactsController
Klasa odpowiada za wszelkie operację sieciowę na kontaktach z bazy danych. Wszystkie
metody sieciowe oprócz Get() wymagają autoryzacji tokenem.
Metody: Get() - zwraca wszystkie kontakty
GetById(int id) - zwraca jeden kontakt o danym id
Create(Contact contact) - tworzy nowy kontakt w bazie danych
Update(int id, Contact contact) - nadpisuje dane kontaktu o danym id
Delete(int id) - usuwa dany kontakt
IsPasswordValid(string password) - sprawdzenie czy dane hasło jest wystarczająco silne

Klasa: AuthController
Klasa odpowiada za autoryzację użytkownika w panelu logowania.
Metody: GenerateJwtToken(User user) - tworzy nowy token uwierzytelniający użytkownikowi
który będzie aktualny przez godzinę
Login(LoginDto dto) - sprawdza czy dane podane podczas logowania zgadzają się z danym
użytkownika w programie

Klasa: AppDbContext
Klasa odpowiada za konfigurowanie relacji między tabelami.
Metody: OnModelCreating(ModelBuilder modelBuilder) - robi dokładnie to co cała metoda.


Modele:
User - do logowania w celu używania wszystkich funkcjonalności
Category - kategoria zawierająca nazwę i ewentualnie swoje podkategorie
Subcategory - podkategoria odwołująca się do swojej kategorii
Contact - kontakt zawierający pełnię informacji


Program.cs
Cała logika uruchamiania backendu aplikacji, w kodzie poszczególne etapy mają wpisane
komentarze.


Biblioteki:
BCrypt.Net-Next - biblioteka do haszowania haseł.
Microsoft.AspNetCore.Authentication.JwtBearer - obsługuje weryfikację tokenów JWT
Microsoft.AspNetCore.Identity - współpracuje z Entity Framework Core i JWT
Microsoft.EntityFrameworkCore - ORM dla C# umożliwiający operacje na bazie danych
Microsoft.EntityFrameworkCore.SqlServe - umożliwia komunikację z bazą danych SQL
Server
Microsoft.EntityFrameworkCore.Tools - umożliwia tworzenie migracji, aktualizowanie bazy
danych i generowanie schematu na podstawie modeli.


Sposób kompilacji aplikacji
1. Otworzyć projekt w Visual Studio
2. Skonfigurować połączenie z bazą danych, wykonać migrację w Package Manager
Console
3. Uruchomić aplikację backendową
4. Otworzyć katalog frontendowy w np. Visual Studio Code
5. Uruchomić aplikację w trybie deweloperskim - ‘npm run dev’
6. Wejść na adres podany w terminalu
7. Na stronie można przejść do listy kontaktów jako gość, lub użyć konta admina
którego dane są w bazie danych
