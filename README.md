## Publisher: `TourBooking.Web/Components/Pages/Book.razor`

2 EditForms, one for booking and one for cancelling a booking.

Bookings are stored directly in a list in TourBooking.Web, to keep it as scoped as
possible regarding messaging and topic exchange.

## Consumer: `BackOffice/Program.cs`

- Gets all `tour.something` using `tour.#` as routing key
- Has its own queue: `backoffice-service` connected to the Exchanger --> `tours`

## Consumer: `EmailService/Program.cs`

- Gets only `tour.booked` using `tour.booked` as routing key
- Has its own queue: `backoffice-queue` connected to the Exchanger --> `tours`

The consumers still need the exchange name to make starting order of applications irrelevant (every consumer, declares the exchange if it doesnt already exist)
![BackOffice and EmailService receiving messages](test.png)
