using System.Diagnostics;

const string StudentFile = @"C:\Users\subom\OneDrive\Desktop\student.txt";
const string CourseFile = @"C:\Users\subom\OneDrive\Desktop\courses.txt";
const string GradeFile = @"C:\Users\subom\OneDrive\Desktop\grades.txt";
const string SummaryFile = @"C:\Users\subom\OneDrive\Desktop\summary.txt";

// Sequential Read
var sw = Stopwatch.StartNew();
string studentsRaw = await File.ReadAllTextAsync(StudentFile);
string gradesRaw = await File.ReadAllTextAsync(GradeFile);
string coursesRaw = await File.ReadAllTextAsync(CourseFile);

sw.Stop();

Console.WriteLine($"Elapsed time in sequential read: {sw.ElapsedMilliseconds}ms");

// Concurrent Read
sw.Restart();

Task<string> studentTask = File.ReadAllTextAsync(StudentFile);
Task<string> gradeTask = File.ReadAllTextAsync(GradeFile);
Task<string> courseTask = File.ReadAllTextAsync(CourseFile);

string[] results = await Task.WhenAll(studentTask, gradeTask, courseTask);

sw.Stop();

Console.WriteLine($"Elapsed time in concurrent read: {sw.ElapsedMilliseconds}ms");

// Summary Write
List<string> cStudents = ParseLines(results[0]);
List<string> cGrades = ParseLines(results[1]);
List<string> cCourses = ParseLines(results[2]);

IEnumerable<string> summaryLines = cStudents.Zip(cCourses, (student,course) =>(student,course)).
    Zip(cGrades, (pair, grade) => $"{pair.student} | {pair.course} | {grade}");

string summaryContent = string.Join(Environment.NewLine, summaryLines);

await File.WriteAllTextAsync(SummaryFile, summaryContent);

Console.WriteLine($"Summary written to {SummaryFile}");

Console.WriteLine("\n------------Summary.Txt Preview-------------");
Console.WriteLine(summaryContent);


static List<string> ParseLines(string raw) =>
    [.. raw.Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => l.Trim())
    .Where(l => l.Length > 0)];