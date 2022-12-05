# TestsGenerator

Многопоточный генератор шаблонного кода тестовых классов для библиотеки тестирования NUnit. Генерируется по одному пустому тесту на каждый публичный метод тестируемого класса. Учитывается перегрузка методов.

Входные данные:

список файлов для классов, из которых необходимо сгенерировать тестовые классы;
путь к папке для записи созданных файлов;
ограничения на секции конвейера.

Выходные данные:

файлы с тестовыми классами.

Генерация выполняется в конвейерном режиме и состоит из трех этапов:

параллельная загрузка исходных текстов в память (с ограничением количества файлов, загружаемых за раз);
генерация тестовых классов в многопоточном режиме (с ограничением максимального количества одновременно обрабатываемых задач);
параллельная запись результатов на диск (с ограничением количества одновременно записываемых файлов).

Генератор учитывает структуру тестируемого класса:

если тестируемый класс принимает через конструктор зависимости по интерфейсам, то в тестовом классе объявляется метод SetUp, в котором создаются Mock-объекты всех необходимых ему зависимостей, которые сохраняются в поля тестового класса и передаются в конструктор для создания экземпляр тестируемого класса;
генерируется по одному шаблонному тесту на каждый публичный метод тестируемого класса и создаются шаблоны для Arrange, Act, Assert секций метода;
секция Arrange содержит объявление переменных со значениями по умолчанию;
секция Act содержит вызов тестируемого метода с передачей ему аргументов, объявленных в Arrange, и сохранение результата метода в переменную actual;
секция Assert содержит объявление переменной expected с типом, соответствующим возвращаемому значению метода, и одну проверку на равенство actual и expected.
