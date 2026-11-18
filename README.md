# Aureo

A modern, translucent personal finance desktop manager built with **WPF (.NET 10)** and **C#**, following the **MVVM (Model-View-ViewModel)** architectural pattern.

> **Status:** In active development (Building incrementally from architecture foundation to UI polish).

---

## Key Features

- **Multi-Account Management:** Track balances across separate bank accounts (e.g., daily checking vs. savings/reserve) with dedicated inter-account transfers that do not distort income/expense analytics.
- **Income & Yield Tracking:** Categorized income entries plus an on-demand percentage yield calculator applied directly to selected account balances.
- **Hierarchical Expenses & Installments:** Two-level category/subcategory organization (e.g., *Subscriptions → Streaming → Amazon Prime*) with renewal day tracking and an automated installment generator.
- **Dynamic Home Dashboard:** Time-aware greetings and contextual alerts generated from upcoming due dates and active installments.
- **General View & History:** Consolidated net worth overview, monthly evolution charts, and category distribution.
- **Customizable Themes & Local Backup:** Dynamic theme switching via XAML `ResourceDictionary` and automatic local persistence with manual JSON backup/restore.

---

## Tech Stack & Architecture

- **Framework:** .NET 10 (WPF - Windows Presentation Foundation)
- **Language:** C# & XAML
- **Architecture:** MVVM (strict separation between `Models`, `ViewModels`, `Views`, `Services`, and `Themes`)
- **Persistence:** Local storage and backup serialization via `System.Text.Json`
- **Styling:** Custom `ResourceDictionary` tokens using `DynamicResource` for runtime theme switching

---

## Project Structure

```text
Aureo/
├── Models/       # Pure domain entities (UserProfile, BankAccount, Category, Subcategory, Transaction)
├── ViewModels/   # Presentation logic, INotifyPropertyChanged, and ICommand implementations
├── Views/        # XAML windows and modular UserControls
├── Services/     # JSON storage persistence, backup handling, and financial calculators
└── Themes/       # Dynamic ResourceDictionaries (Dark/Light palettes, translucent surfaces, and pill-shaped styles)
```

---

## Development Roadmap

- [x] **Phase 0:** MVVM folder structure and dynamic theme dictionary (`DarkTheme.xaml`)
- [ ] **Phase 1:** Core domain models and local JSON persistence (`StorageService`)
- [ ] **Phase 2:** Main shell layout, floating pill navigation dock, and dynamic greeting view
- [ ] **Phase 3:** User profile, multi-bank CRUD, inter-account transfers, and manual backup
- [ ] **Phase 4:** Category carousel selector, animated subcategory bottom sheet, and installment generator
- [ ] **Phase 5:** General view analytics and monthly evolution charts
- [ ] **Phase 6:** Smooth `CubicEase` animations, translucent glass refinements, and UI showcase

---

## How to Run

1. Clone this repository:
   ```bash
   git clone [https://github.com/fppmartan/aureo.git](https://github.com/fppmartan/aureo.git)
   ```
2. Open `Aureo.sln` in **Visual Studio 2022/2026** with the **.NET Desktop Development** workload installed.
3. Press **F5** (or click **Start**) to build and run the application.
