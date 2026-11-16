# CarRental

Консольний застосунок, який веде облік прокату автомобілів однієї філії: бронювання, видачу,
повернення зі штрафом за запізнення, пошук оренд і звіт про зайнятість автопарку.

## Автор і дисципліна

Варіант № 7 «CarRental». Сергій Зівак, група 471, КІСІТ.
Дисципліна «Конструювання програмного забезпечення», самостійна робота № 3.

## Швидкий старт

```
git clone https://github.com/zivak0707-lang/CarRental.git
dotnet build CarRental/CarRental.sln -c Release
dotnet run --project CarRental/src/CarRental.ConsoleApp
```

Потрібен .NET SDK 8.0 або новіший. Застосунок читає дані з теки `data/` поточного каталогу,
тому з кореня репозиторію достатньо `dotnet run --project src/CarRental.ConsoleApp`.
Журнал роботи пишеться у `logs/carrental.log`. Докладніше — у [docs/user-guide.md](docs/user-guide.md).

## Структура рішення

| Проєкт | Призначення |
|---|---|
| `src/CarRental.Domain` | Договір оренди, його стани й профільне правило розрахунку доби та штрафу. |
| `src/CarRental.Application` | Сценарії UC-01…UC-06, валідація вводу й інтерфейси сховищ. |
| `src/CarRental.Infrastructure` | Зберігання у файлах JSON і журнал у файл. |
| `src/CarRental.ConsoleApp` | Меню, ввід із повтором, форматування виводу й композиційний корінь. |
| `tests/CarRental.Tests` | Модульні й архітектурні тести всіх шарів. |

Шари й правило залежностей описано в [docs/architecture.md](docs/architecture.md); посібник —
`docs/user-guide.md`, журнал рішень — `docs/decisions.md`, тестування — `docs/testing.md`,
самооцінка — `docs/self-check.md`, стандарт кодування — `CODING_STANDARD.md`.

## Можливості

Сценарії докладно описано в [docs/spec.md](docs/spec.md).

| Пункт меню | Сценарій |
|---|---|
| 1 | UC-01 Забронювати автомобіль |
| 2 | UC-02 Видати автомобіль |
| 3 | UC-03 Закрити оренду з розрахунком прокату й штрафу |
| 4 | UC-04 Скасувати бронювання |
| 5 | UC-05 Знайти оренди за станом, клієнтом і номером авто |
| 6 | UC-06 Звіт «Зайнятість автопарку» на дату |

Профільне правило: доба — розпочаті 24 години; пільга 60 хвилин; штраф — 50 % ціни доби
за кожну розпочату добу запізнення понад пільгу.

## Тести й покриття

```
dotnet test -c Release
dotnet test -c Release -tl:off /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage/
reportgenerator -reports:tests/CarRental.Tests/coverage/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

43 тест-методи, 75 прогонів, усі зелені. Покриття бібліотеки логіки (Domain + Application):
рядки 98,6 %, гілки 81,8 %. Пояснення непокритих гілок — у [docs/testing.md](docs/testing.md).

Перевірка, рівноцінна конвеєру CI, однією командою:

```
dotnet format --verify-no-changes && dotnet build -c Release -warnaserror && dotnet test -c Release /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line
```

## Технології

.NET 8, C# 12, xUnit, coverlet, Microsoft.Extensions.Logging, System.Text.Json,
аналізатори Roslyn (вбудовані правила .NET, StyleCop.Analyzers, BannedApiAnalyzers),
GitHub Actions (`.github/workflows/ci.yml`, сім кроків, поріг рядкового покриття 70 %).

## Обмеження й відомий борг

Застосунок однокористувацький: файли JSON читаються й переписуються цілком, тому дві
копії, запущені одночасно, перезапишуть зміни одна одної. Видачу авто дозволено й до початку
заброньованого періоду (борг TD-10). Повідомлення про заборонену дію показує стан договору
англійською (`Closed`), а журнал пише дату й суми в інваріантному форматі (борг TD-12).
Оплати, депозиту, продовження оренди й реєстрації нових клієнтів застосунок не підтримує.
