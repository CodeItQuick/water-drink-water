# water-drink-water

[![Run Unit Tests](https://github.com/tbd-friends/water-drink-water/actions/workflows/ci.yml/badge.svg)](https://github.com/tbd-friends/water-drink-water/actions/workflows/ci.yml)

An application to help you remember to drink water.

This accompanies streams on https://twitch.tv/tbdgamer so, there are a few guidelines to follow with PRs.

## Guidelines

- No major stand-alone PRs, i.e. Don't do a PR with "I made it DDD or Here's the project implemented with the 'right' architecture"
- You're more than welcome to pick up existing issues and make a fix for them.
- If you feel you found a bug or an issue, create an issue on the GH board, and create a PR for the issue.
- Welcome to make alternative UI, add under clients folder name <technology.initials>
  - Must not change API to accommodate (other than CORS)
- All PRs will be reviewed on stream
- There's no "right" or "wrong", "bad" or "good" but making a PR doesn't mean it will get merged.
- These guidelines aren't final
- Have an idea for the project, a problem or a challenge? Let's discuss https://github.com/tbd-friends/water-drink-water/discussions/

## Getting Started

- Fork the project, this will create a version of the repository in your GitHub account. You can then work on this, make changes, branches etc. When you're ready, you'll be able to make a Pull-Request from your repository to the main repository.(If you just checkout the project, you won't be able to push any changes you make)
- Open the solution file.
- If you are using Visual Studio you may need an extension to view the database, which is SQLite. [SQLiteBrowser](https://sqlitebrowser.org/dl/)
- Configure the Startup Projects.
  - Select Multiple startup projects, right click on the solution.
  - Set the 'api' and 'blazor.wa.tbd' projects to Action == Start
- Start the application
- Navigate to the api folder and open the 'createTestAccount.http' file.
- Bring the Debug Console Window into view, check it's running on the same port as what is specified in the 'createTestAccount.http' file, if not update it, save, then just above the word "POST", click "Send Request", you should see the record created.
- Now check the Blazor Debug Console and navigate to the url, log in using the username '<test@nowhere.com>' and the password 'test'
- You must be thirsty after that, drink water 😀

## So, you found an issue?

- Check the issues board, see if it's already reported there. 
- If not, create a new issue and be as detailed as you can as to the problem. Screenshots / Steps to reproduce are always helpful.
- If you're able to fix the issue, follow the steps to fork the repository and make a PR with the fix.
- Don't forget there are unit tests, if your code changes existing code, there may be unit tests to update. 
  - DO NOT be afraid if you've not done unit testing before, just reach-out (either on GitHub or in the Discord) and ask for a little assistance.- Don't worry if your fix breaks unit tests, we can work through that together on-stream and get them passing. What's more important is the experience, nothing is broken!- 
