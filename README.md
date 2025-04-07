# LotteryGame

This repository contains a single Visual Studio Solution file with the following projects:

- LotteryGame.Console
- LotteryGame.Shared
- LotteryGame.UnitTests

## LotteryGame.Console
Contains classes which will play a game of Lottery for a player against a random number of CPU Players. This works as a console application and uses input from the user to decide how many tickets to buy, and outputs the game result to the console window

## LotteryGame.Shared

Contains shared components/services used in the lottery game logic, including the following:

- `TicketService` - Contains some helper methods to flatten the `Players` array into a single array of tickets, and also used for validating the purchasing of tickets against balance
- `RandomGenerator` - very simple class with a single method to return a random number between two values (inclusive). Designed this way so that the random element can be removed in Unit Tests using Mocks
- `GameLogicService` - Contains the vast majority of game logic in the `GenerateResult` method

The following Utilities are also contained in this project:

- `CurrencyHelper` - useful for formatting an integer currency which is stored as cents into a nicely formatted string with a currency symbol using the value in the app settings

- `Extensions` - Contains a version of the fisher-yates shuffle algorithm to randomly shuffle an array of any type

## LotteryGame.UnitTests
Contains all Unit tests for the project, which use the XUnit framework and make use of the Moq library to mock services which are not in use for each test, so services can be tested independently

## Configuration

Configuration for the applicaton can be edited in the `appsettings.json` file inside the `LotteryGame.Console` project

The following settings are available, all of which are nested inside the `LotteryGameSettings` object so they can be tightly bound to a C# class:

(Note) All settings are mandatory (using the DataAnnotations Attributes) therefore the application will throw an exception upon startup if any required settings are missing

- `CurrencySymbol` - Set this to whichever currency you wish to use (defaults to USD)
- `MinNumberOfPlayers` - the minimum number of CPU players that will be randomly generated for a game
- `MaxNumberofPlayers` - the maximum number of CPU players that will be randomly generated for a game
- `MinNumberOfTicketsPerPlayer` - the smallest amount of tickets users can buy. This is used both for validation for the player's choice, and for the minimum random value chosen for CPU Players
- `MaxNumberOfTicketsPerPlayer` - the largest amount of tickets users can buy. Used for both validation of the player's choice and for the maximum random vlaue chosen for CPU players
- `PlayerStartingBalance` - the default starting balance for the player controlled user. Note the value is measured in cents (default 1000, or $10.00)
- `CostPerTicket` - the price (in cents) of each ticket
- `PrizeSettings` - this is an array of the prize levels associated with the game. You can add or remove prize levels if needed to extend the game. The following settings are used to control prizes:
  - `Name` - A string representation of the prize name, which is used as the prize key so should be unique
  - `NumberOfWinningTickets` - The number of tickets that should win this prize
  - `PercentageOfWinningTickets` - the percentage (expressed as a decimal) of how many tickets should be drawn for this prize. Note that this setting **or** `NumberOfWinningTickets` should be set for a prize, but not both

## FutureWork/Potential Improvements

Some potential improvements or further additions of the code include:
* More prizes can be added using the included prize structure in the `LotteryGameSettings` class and adding to the `appsettings.json` file
* It would be possible to use the existing `Shared` library project with other deployable projects with no change. For example, a .NET MVC or Blazor application could include this library and then use the `GameResult` object to display the results of games directly in a Browser
* Unit Tests of the `ConsoleLotteryGame` class could be added
* Integration tests of the `LotteryGame.Console` application could be created to provide a full end-to-end test of the logic including the View components