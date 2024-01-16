public interface IUserAction
{
    void AddUser(User user);
    List<User> GetUserList();
    void DeleteUser(string phoneNumber);
    List<User> GetUserByFilter(string filter);
}

public class UserAction : IUserAction
{
    private List<User> userList;
    private string userFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "users.txt");

    public UserAction()
    {
        userList = new List<User>();
        LoadUsersFromFile();
    }

    public void AddUser(User user)
    {
        if (!userList.Any(u => u.PhoneNumber == user.PhoneNumber))
        {
            userList.Add(user);
            SaveUsersToFile();
            Console.WriteLine("Kullanıcı Eklendi");
        }
        else
        {
            Console.WriteLine("Kullanıcı zaten kayıtlı");
        }
    }

    public void DeleteUser(string phoneNumber)
    {
        var userToRemove = userList.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
        if (userToRemove != null)
        {
            userList.Remove(userToRemove);
            SaveUsersToFile();
            Console.WriteLine("Kullanıcı başarıyla silindi.");
        }
        else
        {
            Console.WriteLine("Belirtilen telefon numarasına sahip bir kullanıcı bulunamadı.");
        }
    }

    public List<User> GetUserByFilter(string filter)
    {
        return userList.Where(u => u.Name.Contains(filter) || u.Surname.Contains(filter) || u.Email.Contains(filter) || u.PhoneNumber.Contains(filter)).ToList();
    }

    public List<User> GetUserList()
    {
        return userList;
    }

    private void LoadUsersFromFile()
    {
        if (File.Exists(userFilePath))
        {
            var lines = File.ReadAllLines(userFilePath);
            foreach (var line in lines)
            {
                var userProperties = line.Split('#');
                var user = new User
                {
                    UserId = Guid.Parse(userProperties[0]),
                    Name = userProperties[1],
                    Surname = userProperties[2],
                    Email = userProperties[3],
                    PhoneNumber = userProperties[4],
                    IsAdmin = bool.Parse(userProperties[5]),
                    Password = userProperties[6]
                };
                userList.Add(user);
            }
        }
        else
        {
            File.Create(userFilePath).Close();
        }
    }

    private void SaveUsersToFile()
    {
        using (StreamWriter writer = new StreamWriter(userFilePath))
        {
            foreach (var user in userList)
            {
                writer.WriteLine($"{user.UserId}#{user.Name}#{user.Surname}#{user.Email}#{user.PhoneNumber}#{user.IsAdmin}#{user.Password}");
            }
        }
    }
}

public interface INoteAction
{
    void AddNote(Note noteText);
    List<Note> GetNoteList(Guid userId);
}

public class NoteAction : INoteAction
{
    private List<Note> noteList;
    private string noteFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "notes.txt");

    public NoteAction()
    {
        noteList = new List<Note>();
        LoadNotesFromFile();
    }

    public void AddNote(Note newNote)
    {
        string formattedNote = $"{newNote.NoteId}#{newNote.UserId}#{newNote.NoteText}#{DateTime.UtcNow:dd.MM.yyyyTHH:mm:ssZ}";
        //noteList.Add(formattedNote);
        SaveNotesToFile(formattedNote);
        Console.WriteLine("Not başarıyla eklendi.");
    }

    public List<Note> GetNoteList(Guid userId)
    {
        return noteList.Where(n => n.UserId == userId).ToList();
    }

    private void LoadNotesFromFile()
    {

        if (File.Exists(noteFilePath))
        {
            var lines = File.ReadAllLines(noteFilePath);
            var notestring = String.Join("", lines);
            var notes = notestring.Split("###");
            foreach (var item in notes)
            {
                var lineitems = item.Split('#');
                Note note = new Note();
                note.NoteId = new Guid(lineitems[0]);
                note.UserId = new Guid(lineitems[1]);
                note.NoteText = lineitems[2];
                note.NoteDate = lineitems[3];
                noteList.Add(note);

            }

        }
        else
        {
            File.Create(noteFilePath).Close();
        }
    }

    private void SaveNotesToFile(string line)
    {
        File.WriteAllText(noteFilePath, line);
    }
}
public class Note
{
    public Guid NoteId { get; set; }
    public Guid UserId { get; set; }
    public string NoteText { get; set; }
    public string NoteDate { get; set; }

}

public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsAdmin { get; set; }
    public string Password { get; set; }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Mail Giriniz:");
        string email = Console.ReadLine();

        Console.WriteLine("Şifre Giriniz:");
        string password = Console.ReadLine();

        if (email.ToLower() == "admin@example.com" && password == "admin")
        {
            AdminMenu();
        }

        UserAction userAction = new UserAction();
        var user = userAction.GetUserList().FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            if (user.IsAdmin)
            {
                AdminMenu();
            }
            else
            {
                UserMenu(user.UserId);
            }
        }
        else
        {
            Console.WriteLine("Geçersiz kullanıcı adı veya şifre.");
        }
    }

    static void AdminMenu()
    {
        UserAction userAction = new UserAction();
        while (true)
        {
            Console.WriteLine("1- Kullanıcı Ekle");
            Console.WriteLine("2- Kullanıcı Ara");
            Console.WriteLine("3- Kullanıcı Sil");
            Console.WriteLine("0- Çıkış");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        // Kullanıcı ekleme
                        Console.Write("İsim:");
                        string name = Console.ReadLine();
                        Console.Write("Soyisim:");
                        string surname = Console.ReadLine();
                        Console.Write("Telefon:");
                        string phoneNumber = Console.ReadLine();
                        Console.Write("Email:");
                        string userEmail = Console.ReadLine();
                        Console.Write("İsAdmin True/False");
                        bool isAdminUser = bool.Parse(Console.ReadLine());
                        Console.Write("Password");
                        string passWord = Console.ReadLine();

                        User newUser = new User
                        {
                            UserId = Guid.NewGuid(),
                            Name = name,
                            Surname = surname,
                            PhoneNumber = phoneNumber,
                            Email = userEmail,
                            IsAdmin = isAdminUser,
                            Password = passWord
                        };
                        userAction.AddUser(newUser);
                        break;

                    case 2:
                        // Kullanıcı arama
                        Console.Write("Arama metni (en az 3 karakter): ");
                        string filter = Console.ReadLine();

                        List<User> searchResults = userAction.GetUserByFilter(filter);

                        if (searchResults.Count > 0)
                        {
                            foreach (var result in searchResults)
                            {
                                Console.WriteLine($"{result.Name} {result.Surname} {result.Email} {result.PhoneNumber} {result.IsAdmin}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Belirtilen kriterde kullanıcı bulunamadı.");
                        }
                        break;

                    case 3:
                        // Kullanıcı silme
                        Console.Write("Telefon numarası: ");
                        string deletePhoneNumber = Console.ReadLine();
                        userAction.DeleteUser(deletePhoneNumber);
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Geçersiz seçim.");
            }
        }
    }

    static void UserMenu(Guid userId)
    {
        while (true)
        {
            Console.WriteLine("1- Not Ekle");
            Console.WriteLine("2- Notlarımı Listele");
            Console.WriteLine("0- Çıkış");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                NoteAction noteAction = new NoteAction();
                UserAction userAction = new UserAction();

                var user = userAction.GetUserList().FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    switch (choice)
                    {
                        case 1:
                            // Not ekleme
                            Console.Write("Not: ");
                            string noteText = Console.ReadLine();

                            Note newNote = new Note
                            {
                                NoteId = Guid.NewGuid(),
                                UserId = userId,
                                NoteText = noteText
                            };
                            noteAction.AddNote(newNote);
                            break;

                        case 2:
                            // Not listeleme
                            NoteAction noteActionList = new NoteAction();
                            List<Note> notes = noteActionList.GetNoteList(userId);

                            if (notes.Count > 0)
                            {
                                foreach (var note in notes)
                                {
                                    Console.WriteLine(note.NoteText);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Henüz not eklenmemiş.");
                            }
                            break;

                        case 0:
                            return;

                        default:
                            Console.WriteLine("Geçersiz seçim.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz kullanıcı adı.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Geçersiz seçim.");
            }
        }
    }
}