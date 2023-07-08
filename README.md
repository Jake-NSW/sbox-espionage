# Espionage

The stealth oriented tactical shooter experience. This is the public code base. Assets are downloaded and used from a
private repo.

## Dependencies

Currently the only dependency is the Signals library. s&box should automatically down it as its in the `.addon` file.

### Signals

Signals is a library that allows for easy event handling. It is used in Espionage to handle events such as player death,
a client connecting to the server, someone dropping or deploying an item, etc. It allows us to easily add logic and
features to the game without directly impacting existing code potentially causing bugs, which is perfect for a game like
Espionage as its a systematically complicated game. You can find the Signals
library [here](https://github.com/Jake-NSW/Signals)