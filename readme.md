# ACST.AWS.Textract

ACST AWS Textract Claims processing utilities

# Requirements

- [Newtonsoft](https://github.com/JamesNK/Newtonsoft.Json)
- [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)

# Features

## Table of Contents

- [Issues and Pull Requests](#issues-and-pull-requests)

## Issues and Pull Requests

- If you're not sure about adding something, [open an issue](https://github.com/Phaedrus-314/ACST.AWS.Textract/issues/new) to discuss it.
- Feel free to open a Pull Request early so that a discussion can be had as changes are developed.
- Include screenshots and animated gifs of your changes whenever possible.

## Git Notes

```git
git init
git remote add origin https://github.com/Phaedrus-314/ACST.AWS.Textract.git
git remote -v
git add .
git commit -m "initial commit"
git push origin master
```

<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://example.com)

There are many great README templates available on GitHub; however, I didn't find one that really suited my needs so I created this enhanced one. I want to create a README template so amazing that it'll be the last one you ever need -- I think this is it.

Here's why:

- Your time should be focused on creating something amazing. A project that solves a problem and helps others
- You shouldn't be doing the same tasks over and over like creating a README from scratch
- You should implement DRY principles to the rest of your life :smile:

Of course, no one template will serve all projects since your needs may be different. So I'll be adding more in the near future. You may also suggest changes by forking this repo and creating a pull request or opening an issue. Thanks to all the people have contributed to expanding this template!

Use the `BLANK_README.md` to get started.

<p align="right">(<a href="#top">back to top</a>)</p>

## Built With

This section should list any major frameworks/libraries used to bootstrap your project. Leave any add-ons/plugins for the acknowledgements section. Here are a few examples.

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [AWS Textract](https://docs.aws.amazon.com/textract/latest/dg/sdk-general-information-section.html)
- [Entity Framework](https://docs.microsoft.com/en-us/ef/)
- [Newtonsoft](https://github.com/JamesNK/Newtonsoft.Json)
- [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)

<p align="right">(<a href="#top">back to top</a>)</p>

## Prerequisites

Before you can run an AWS Textract application, you have to configure your environment.

To configure your environment

Create or update an IAM user with AmazonTextractFullAccess permissions. For more information, see [Step 1: Set Up an AWS Account and Create an IAM User.](https://docs.aws.amazon.com/textract/latest/dg/setting-up.html#setting-up-iam)

Install and configure the AWS CLI and the AWS SDKs. For more information, see [Step 2: Set Up the AWS CLI and AWS SDKs.](https://docs.aws.amazon.com/textract/latest/dg/setup-awscli-sdk.html)

For a quick deploy to a new development PC, create .aws folder under users profile and add two files:

- [config](#config)
- [credentials](#credentials)

> ## config
>
> ```
> [default]
>
> region = us-east-1
>
> output = json
>
> logging = console
> ```
> 
> ## credentials
>
> 
> Amazon Web Services Credentials File used by AWS CLI, SDKs, and tools
>
> This file was created by the AWS Toolkit for Visual Studio Code extension.
>
> Your AWS credentials are represented by access keys associated with IAM users.
> For information about how to create and manage AWS access keys for a user, see:
> <https://docs.aws.amazon.com/IAM/latest/UserGuide/id_credentials_access-keys.html>
>
> This credential file can store multiple access keys by placing each one in a
> named "profile". For information about how to change the access keys in a
> profile or to add a new profile with a different access key, see:
> <https://docs.aws.amazon.com/cli/latest/userguide/cli-config-files.html>
>
> **[awsBrian]**
>
> The access key and secret key pair identify your account and grant access to AWS.
**aws_access_key_id** = AKIAWR2XXXXXXXE5I
>
> Treat your secret key like a password. Never share your secret key with anyone. Do not post it in online forums, or store it in a source control system. If your secret key is ever disclosed, immediately use IAM to delete the access key and secret key and create a new key pair. Then, update this file with the replacement key details.
>
> **aws_secret_access_key** = IuiETe0oxxxxxxxxxxxxxxxxxxxxxplV
>
> **toolkit_artifact_guid** = 908a6dca-aaaa-bbbb-nnnn-6417fb98azzz
>
> **[default]**
>
> **aws_access_key_id** = AKIAWR2KGYNH6VBS6E5I
>
> **aws_secret_access_key** = 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
>
> **toolkit_artifact_guid** = a4aa6408-f5cb-4712-be40-4def8c489e7c

<p align="right">(<a href="#top">back to top</a>)</p>

### Learning Resources

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.

- npm

  ```sh
  npm install npm@latest -g
  ```

  ```
