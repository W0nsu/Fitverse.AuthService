# OPIS

Fitverse jest dwuosobowym projektem realizowanym w ramach pracy inżynierskiej.
Zamysłem było stworzenie systemu, którego funkcjonalności działają niezależnie od siebie. Wdrożenia nie trwają cały dzień, a system jest w stu procentach sprawny w trakcie ich trwania. Nie straszny nam jest też zwiększony ruch na maszynach, ponieważ jest on sprawnie rozkładany przez load balancer Kubernetes'a.

Fitvers jest narzędziem umożliwiającym zarządzanie klubem fitness - w skład jego funkcjonalności wchodzą:
- Budowanie i zarządzanie bazą klitentów
- Dodawanie do ich profili umów tworzonych na podstawie wcześniej zdefiniowanych szablonów
- Weryfikowanie czasu trwania umów oraz śledzenie terminów płatności
- Tworzenie grafiku oraz zapis klientów na zajęcia

Na system składają się cztery mikrousługi o następujących funkcjonalnościach:
- Members service - Budowanie i zarządzanie bazą klitentów(Najprościej mówiąc CRUD) oraz dodawanie do profili umów oraz wyświetlanie podstawowych informacji z nimi związanymi
- Agreements service - Wyświetelanie szczegółowych informacji dotyczących umów, płatność za raty, dodawanie szablonów umów członkowskich(Przykładowo: karnet open na 12 miesięcy, cena raty 100zł - na podstawie tego szablonu dodawana jest umowa do profilu z konkretnymi danymi, czyli od kiedy do kiedy oraz ile ma rat)
- Calendar service - Tworzenie nowych typów zajęć, tworzenie na ich podstawie terminarzy oraz zapis klientów na utworzone wcześniej zajęcia
- Authorization service - Autoryzacja użytkowników systemu :)

Każda z mikrousług posiada własną bazę danych, a co za tym idzie każda z nich jest całkowicie niezależna od pozostałych. Komunikacja między usługami odbywa się za pomocą message brokera RabbitMQ.

Wszystko razem spina frontend stworzony w Blazor Webassembly.

# TECHNOLOGIE
### IMPLEMENTOWANE PRZEZ MNIE
- .Net Core 5
  - MediatR
  - Entity Framework
  - Autoryzacja JWT
  - FlientValidation
  - ProblemDetails
- Blazor Webassembly
- RabbitMQ

### IMPLEMENTOWANE PRZEZ DRUGIEGO CZŁONKA ZESPOŁU

- Docker
- Kubernetes
- Azure Cloud
- Terraform
- Azure CI/CD
