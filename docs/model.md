# Класова модель варіанта № 7 (CarRental)

Документ — бронювання (Reservation), позиція — автомобіль на період дат
(ReservationDay), учасник — орендар (Renter), об'єкт обліку — автомобіль (Car).

## Сутності

| Сутність | Роль | Незмінний | Інваріант |
|---|---|---|---|
| ReservationDay | позиція документа | так, record | Plate не порожній; EndDate > StartDate; DailyRate > 0 |
| Reservation | документ зі станом | ні, агрегат | Id > 0; RenterId > 0; позиції додаються лише у стані Draft |
| Renter | довідник учасників | так, record | Id > 0; FullName не порожнє; Email містить @ |
| Car | довідник об'єктів обліку | ні (IsAvailable змінюється) | Plate не порожній; DailyRate > 0 |

## Стани бронювання

| Зі стану | До стану | Умова |
|---|---|---|
| Draft | Confirmed | є хоча б одна позиція |
| Draft | Cancelled | завжди |
| Confirmed | Active | завжди (автомобіль видано клієнтові) |
| Confirmed | Cancelled | завжди (до видачі автомобіля) |
| Active | Closed | завжди (автомобіль повернуто) |

Closed і Cancelled — кінцеві стани: з них переходів немає.