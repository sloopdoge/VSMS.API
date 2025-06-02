# 🧠 VSMS.API — Virtual Stock Market Simulator API

**VSMS** is a real-time, educational simulator of the stock market built with **.NET**, **Blazor**, **SignalR**, and a full **microservice architecture**. It allows users to trade virtual stocks, track their portfolio, and compete on a leaderboard — all while learning about financial systems.

---

## 🌟 Features

- 🔐 JWT-based authentication and role-based authorization
- 📈 Real-time stock price simulation with SignalR
- 🏢 Company and stock management for admins
- 💸 Virtual trading system (Buy/Sell)
- 📊 Portfolio overview with profit/loss tracking
- 📉 Historical performance charts
- 🏆 Leaderboard to rank user performance

---

## 🧱 Microservices Overview

- **IdentityService** – handles auth, registration, JWTs
- **CompanyService** – manages companies
- **StockService** – manages and simulates stock prices
- **TradeService** – handles buying/selling operations
- **PortfolioService** – tracks user holdings and value
