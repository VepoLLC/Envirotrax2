## Umbraco Login
This is an Umbraco app. To login to the backoffice, please navigate to /umbraco URL and use the following credentials:

- Username: hayk.shirinyan@developerpartners.com
- Passsword: P@ssw0rd!!

## Azure Resource Access
This project uses Managed Identities for accessing Azure resources such as Key Vaults and Storage Accounts. Please open a terminal and type the following command before launching the project:

`az login --allow-no-subscriptions`

## Images
When you need images to use in the project, please download the images the following sources:
1. The V1 project website.
2. Pixabay https://pixabay.com/
3. Unsplash https://unsplash.com/

All those would be free images.

## Front-End Libraries
This project uses Libman for managing front-end libraries. Please open the folder where the .csproj file is and run the following command in terminal to download the CSS and JS packages:

`libman restore`


## Claude Code Usage
I configured Umbraco to use SourceCodeAuto models builder. Normally, when you find Umbraco documentation, all their examples access content like this Model.Value<string>("YourAliasName"). However, with SourceCodeAuto models builder, it generates strongly typed MVC view, so you access the page models like regular C# models like Model.PageTitle. 

That allows Claude Code to read the C# code models generated in the Umbraco/Models folder to know what the data types of each pages are. So you can easily work with Claude code because it know all the document types and their properties you created in Umbraco admin dashboard.

You can learn more about Models Builder here:
https://docs.umbraco.com/umbraco-cms/develop-with-umbraco/templating-and-rendering/templating/modelsbuilder/builder-modes
