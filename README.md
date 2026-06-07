# Student Planner

Student Planner este o aplicatie desktop realizata in C# folosind Avalonia UI si arhitectura MVVM. Aplicatia ajuta studentii sa isi organizeze taskurile, sesiunile de invatat si timpul de lucru printr-un timer de tip Pomodoro.

## Tehnologii folosite

- C#
- .NET
- Avalonia UI
- MVVM
- JSON pentru salvarea datelor locale
- Git si GitHub pentru versionare

## Functionalitati principale

1. Adaugare taskuri pentru facultate
2. Stergere taskuri
3. Marcarea taskurilor ca finalizate
4. Cautare taskuri dupa titlu
5. Filtrare taskuri dupa status
6. Salvarea si incarcarea taskurilor din fisier JSON
7. Adaugare si stergere sesiuni de invatat
8. Salvarea sesiunilor de invatat in JSON
9. Dashboard cu statistici pentru taskuri si sesiuni
10. Timer Pomodoro personalizabil

## Structura aplicatiei

Aplicatia este organizata pe mai multe foldere:

- `Models` - clasele care definesc datele aplicatiei
- `Services` - clasele care se ocupa de salvarea si incarcarea datelor din JSON
- `ViewModels` - logica aplicatiei si comenzile folosite in interfata
- `Views` - interfata grafica realizata cu Avalonia XAML

## Arhitectura MVVM

Proiectul foloseste arhitectura MVVM:

- Model: reprezinta datele aplicatiei, precum taskurile si sesiunile de invatat
- View: reprezinta interfata grafica a aplicatiei
- ViewModel: face legatura dintre interfata si logica aplicatiei

## Element nou adaugat

Elementul nou adaugat este un timer Pomodoro personalizabil. Utilizatorul poate introduce durata dorita pentru sesiunea de invatat, poate porni timerul, il poate pune pe pauza si il poate reseta.

## Cum se ruleaza aplicatia

Aplicatia se ruleaza din terminal cu:

```bash
dotnet run