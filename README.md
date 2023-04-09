# ://[mikemrobinsondev.com](http://www.mikemrobinsondev.com) codebase.

This is a project written in ASP.NET 7. The purpose of sharing this with the GitHub community is to show the code that I crafted.

I originally built this project because I wanted to start a blog to chronical my coding Journy. So I thought, if I am serious about coding, why not make my first major project a small custom CMS? I am constantly improving this work, so it should be no surprise if the way I write the code changes.

# Features

- Administrators can create, edit, and delete blogs.

- Administrators can create, edit, and delete posts and share them on chosen social media platforms.

- Users can create comments for posts and administrators can moderate comments.

- Administrators can create projects and upload multiple images to them. Users can share projects on chosen social media platforms.

- Users can create an account, upload an avatar, and change account information (needed to post comments).


# Setting up the development environment

To run a local copy of the code, please make sure you have the following packages installed:

[Microsoft .NET 7 Runtime X64](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

[Entity Framework Core v6.2.0+]()

[Oracle MySQL v8.0.32+](https://dev.mysql.com/downloads/)

# Running the software

## A Word of warning

The purpose of making this repo public is perusal. There are external services that are needed and errors will occur if you try to run this code without them. If you want to set up these services to get a copy of this code running on your machine, please follow the steps below.

1.  Create an empty directory to hold the application (the directory I've chosen can be seen in the screenshot below).
    ![Screenshot One](/docpics/img.png)

2.  In the directory you've created, add a folder named `ArticleImages` this is case-sensitive (see below).
    ![Screenshot Two](/docpics/img2.png)

3.  After you've created the `ArticleImages` folder, unzip the release into the folder you've made to hold the application (shown below).
    ![Screenshot Three](/docpics/img3.png)

4.  Inside of the release folder, edit the accompanying appsettings.json (See below, and replace information in brackets with your information, please do not include the brackets when entering your info 😁).

    ```JSON
    {
      "ConnectionStrings": {
        "Production": "server=[MySQL Server IP Address];userid=[MySQL UserName];pwd=[MySQL Password];port=3306;database=portfolio;"
      },
    
      "GoogleAccount": {
        "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        "client_id": "[Your Google Client ID]",
        "client_secret": "[Your Google Secret]",
        "project_id": "[Your Google Client ID]",
        "redirect_uris": ["Your redirect URI"],
        "token_uri": "https://oauth2.googleapis.com/token"
      },
    
      "AppSettings": {
        "MailSettings": {
          "DisplayName": "[Your Display Name]",
          "Host": "[Your SMTP Server]",
          "Mail": "[Your SMTP Email Address]",
          "Password": "[Your SMTP Email Password]",
          "Port": 587
        }
      },
    
      "ReCaptcha": {
        "SiteKey": "[Your Recaptcha Site Key]",
        "SecretKey": "[Your Recaptcha Secret]",
        "Version": "v2",
        "UseRecaptchaNet": false
      }
    }
    ```

5.  Once your json file is correct and the prerequisites have been installed, run the accompanying executable called, `efbundle.exe` this file will automatically setup the database.

6.  With the database built run the app by executing the following command `dotnet exec portfolio.dll`, you should output similar to the screenshot below. Look for the a URL with localhost (shown below), and type it into your browser. You should have a live version of the website running locally.

![ShieldOne](https://img.shields.io/github/license/michael-m-robinson/Portfolio?style=flat-square)![ShieldTwo](https://img.shields.io/github/issues/michael-m-robinson/Portfolio?style=flat-square) 
![ShieldThree](https://img.shields.io/twitter/follow/MichaelMRobins4?style=social)




    





