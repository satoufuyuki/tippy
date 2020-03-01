<h1 align="center">Tippy Discord Bot</h1>

## Building the Bot

### Prerequisite:

You will need the following to utilize all of Tippy's features (items marked with a star are optional):
* MongoDB
* [Emilia API key](https://emilia.shrf.xyz/)
* Other unmentioned API keys*

**We will not provide any support whatsoever in obtaining any of the above.**

<sub>Note: The bot does not necessarily need these keys to function, but some functionality might be limited by the lack of them (ex. without Emilia's API keys, you can't use the action commands). Due to the closed nature of that API, we encourage you to submit a patch that would allow custom images to be used on self-hosted instances if you'd like (ex. by pushing your own -local- API server), but keep them in line with the rest of the code.</sub> 


### Steps for building:
<sub>Please do note that you will not receive any help whatsoever while trying to build your own Tipppy.</sub>
1.  Make sure you have the prerequisites installed and running.
2.  Clone this repository (you can also fork this repo and clone your fork). 
3.  Fill the prerequisites in `AssetsExample.cs`
4.  Rename `AssetsExample` file to `Assets.cs`. Also, rename the class name too.
5.	Open a Terminal in the root folder.
6.  Run `dotnet build`
7.  Change directory to `Tippy` dir
8.	Run the bot using `dotnet run`

