# GIT OVERVIEW AND SETUP INSTRUCTIONS

***Authored by Samuel Kennedy - Ske131*** \
***Last Updated: 2025-07-16***

> Include this file in the root directory of your repository for quick access, or follow the [Git Repository Link](https://github.com/SaxySam/Git_Instructions) for the most recent update.

* **Disclaimer**: This is by no means an exhaustive list; however, it should get you started using git and solving some of the most common problems encountered.

---

## Table of Contents

**Select any of the below links to navigate to that section of this document**

> You can also execute *`Control + T | Command + T`* within Visual Studio Code or select the search bar at the top of the window to view and navigate different sections of this document.

<details open>
<summary><b> Expand Table of Contents </b></summary>

1. [GIT OVERVIEW AND SETUP INSTRUCTIONS](#git-overview-and-setup-instructions)
2. [Table of Contents](#table-of-contents) - **You are here**
3. [Forward](#forward)
4. [Viewing this Document](#viewing-this-document)
5. [What is Git?](#what-is-git)
    * [How Does Git Work?](#how-does-git-work)
    * [What is a Repository?](#what-is-a-repository)
6. [Pre-Setup](#pre-setup)
    * [Install Git](#install-git)
    * [Install Visual Studio Code](#install-visual-studio-code)
    * [Add a .gitignore](#add-a-gitignore)
7. [EngGit Setup Instructions](#enggit-setup-instructions)
    * [Creating a new Repository on EngGit using Visual Studio Code](#creating-a-new-repository-on-enggit-using-visual-studio-code)
        * [Prefilled Terminal Lines for EngGit](#prefilled-terminal-lines-for-enggit)
8. [GitHub Setup Instructions](#github-setup-instructions)
    * [Creating a new Repository on GitHub using Visual Studio Code](#creating-a-new-repository-on-github-using-visual-studio-code)
        * [Prefilled Terminal Lines for GitHub](#prefilled-terminal-lines-for-github)
9. [Preparing Visual Studio Code for Use with Unity](#preparing-visual-studio-code-for-use-with-unity)
10. [Cloning (Downloading) a Repository](#cloning-downloading-a-repository)
    * [Locating the Repository URL](#locating-the-repository-url)
    * [Cloning Using the Visual Studio Code UI](#cloning-using-the-visual-studio-code-ui)
    * [Cloning Using the Terminal](#cloning-using-the-terminal)
11. [Viewing Uncommitted Git Changes](#viewing-uncommitted-git-changes)
    * [Viewing Git Changes using the Visual Studio Code UI](#viewing-git-changes-using-the-visual-studio-code-ui)
    * [Viewing Git Changes using the Terminal](#viewing-git-changes-using-the-terminal)
12. [Staging and Pushing (Uploading) Changes](#staging-and-pushing-uploading-changes)
    * [Pushing Changes Using the Visual Studio Code UI](#pushing-changes-using-the-visual-studio-code-ui)
    * [Pushing Changes Using the Terminal](#pushing-changes-using-the-terminal)
13. [Un-Staging Changes](#un-staging-changes)
    * [Un-Staging Changes Using the Visual Studio Code UI](#un-staging-changes-using-the-visual-studio-code-ui)
    * [Un-Staging Changes Using the Terminal](#un-staging-changes-using-the-terminal)
14. [Discarding Changes](#discarding-changes)
    * [Discarding Changes Using the Visual Studio Code UI](#discarding-changes-using-the-visual-studio-code-ui)
    * [Discarding Changes Using the Terminal](#discarding-changes-using-the-terminal)
15. [Stashing and Popping Changes](#stashing-and-popping-changes)
    * [Stashing Changes Using the Visual Studio Code UI](#stashing-changes-using-the-visual-studio-code-ui)
    * [Popping Changes Using the Visual Studio Code UI](#popping-changes-using-the-terminal)
    * [Stashing Changes Using the Terminal](#stashing-changes-using-the-terminal)
    * [Popping Changes Using the Terminal](#popping-changes-using-the-terminal)
16. [Pulling Changes](#pulling-changes)
    * [Pulling Using the Visual Studio Code UI](#pulling-using-the-visual-studio-code-ui)
    * [Pulling Using the Terminal](#pulling-using-the-terminal)
17. [Dealing with Clashes and Merge Conflicts](#dealing-with-clashes-and-merge-conflicts)
    * [Dealing with Merge Conflicts using the Visual Studio Code UI](#dealing-with-merge-conflicts-using-the-visual-studio-code-ui)
        * [In-Line Editor Method](#in-line-editor-method)
        * [Three-Way Editor Method](#three-way-editor-method)
18. [Adding Collaborators to a Repository](#adding-collaborators-to-a-repository)
    * [Adding Collaborators on EngGit](#adding-collaborators-on-enggit)
    * [Adding Collaborators on GitHub](#adding-collaborators-on-github)
19. [Changing a Git Commit Message](#changing-a-git-commit-message)
20. [Git Branches](#git-branches)
    * [Creating New Branches](#creating-new-branches)
        * [Local Remote Branch Creation](#local-remote-branch-creation)
            * [Creating a New Branch through the Visual Studio Code UI](#creating-a-new-branch-through-the-visual-studio-code-ui)
            * [Creating a New Branch through the Terminal](#creating-a-new-branch-through-the-terminal)
        * [Host Website Branch Creation](#host-website-branch-creation)
            * [Creating a New Branch on EngGit](#creating-a-new-branch-on-enggit)
            * [Creating a new Branch on GitHub](#creating-a-new-branch-on-github)
    * [Checking for Newly Created branches](#checking-for-newly-created-branches)
    * [Switching Branches](#switching-branches)
        * [Switching Branches using the Visual Studio Code UI](#switching-branches-using-the-visual-studio-code-ui)
        * [Switching Branches using the Terminal](#switching-branches-using-the-terminal)
    * [Renaming Branches](#renaming-branches)
        * [Renaming Branches using the Visual Studio Code UI](#renaming-branches-using-the-visual-studio-code-ui)
        * [Renaming Branches using the Terminal](#renaming-branches-using-the-terminal)
            * [Renaming a Local Branch](#renaming-a-local-branch)
            * [Renaming a Remote Branch](#renaming-a-remote-branch)
    * [Deleting Branches](#deleting-branches)
        * [Deleting Branches Locally](#deleting-branches-locally)
            * [Deleting Branches using the Visual Studio Code UI](#deleting-branches-using-the-visual-studio-code-ui)
            * [Deleting Branches using the Terminal](#deleting-branches-using-the-terminal)
        * [Deleting Branches on a Host Website](#deleting-branches-on-a-host-website)
            * [Deleting Branches on EngGit](#deleting-branches-on-enggit)
            * [Deleting Branches on GitHub](#deleting-branches-on-github)
21. [Configuring Git Large File Systems](#configuring-git-large-file-systems)
22. [Restoring / Uncommiting Committed Changes](#restoring--uncommiting-committed-changes)
23. [Adding a file to the .gitignore](#adding-a-file-to-the-gitignore)
24. [Pushing a Repository to / Hosting on Multiple Remotes](#pushing-a-repository-to--hosting-on-multiple-remotes)
    * [Example: Setting a Remote from EngGit to push to both EngGit and GitHub](#example-setting-a-remote-from-enggit-to-push-to-both-enggit-and-github)
    * [Example: Setting a Remote from GitHub to push to both GitHub and EngGit](#example-setting-a-remote-from-github-to-push-to-both-github-and-enggit)
25. [Migrating a Repository](#migrating-a-repository)
    * [Moving from EngGit to GitHub](#moving-from-enggit-to-github)
    * [Moving from GitHub to EngGit](#moving-from-github-to-enggit)
    * [Updating URL after migration](#updating-url-after-migration)
26. [Merging Multiple Separate Repositories into branches under One Single Repository](#merging-multiple-separate-repositories-into-branches-under-one-single-repository)
27. [Useful Links](#useful-links)
28. [Copyright Information](#copyright-information)

</details>

---

## Forward

This document outlines steps and instructions for using git to manage both solo and collaborative projects, from an explanation and setup to covering some of the most commonly encountered problems. It details instructions using both Visual Studio Code and the terminal, as well as instructions for the two most common repository hosting websites: GitLab (EngGit) and GitHub. Think of it like a cheat sheet or reference for anything git related.

While the Visual Studio Code implementation provides a very helpful visual guide, the terminal commands often allow more control and flexibility. As such, some sections of this document are only able to be followed using the terminal, as the UI lacks a direct counterpart.

> Note: Only some Visual Studio Code steps may be able to be recreated within other IDEs, while *all* terminal commands will be able to run within other IDEs or outside of one entirely. Keep this in mind when using different or unfamiliar software.

---

## Viewing this Document

**To view this file properly, please open it in a Markdown format file viewer, such as [*Obsidian*](https://obsidian.md/). An *online Markdown file viewer* can be [found here](https://markdownlivepreview.com/), and an *extension for Visual Studio Code* can be [found here](https://marketplace.visualstudio.com/items?itemName=shd101wyy.markdown-preview-enhanced). [*JetBrains Rider*](https://www.jetbrains.com/rider/download/) also has a built-in Markdown previewer similar to the VSCode extension.**

***Each Markdown viewer has its own quirks and handling methods, and as such, you may encounter slight version differences depending on the software you use.***

* **GitHub** and **GitLab** both have built-in Markdown previews for files named "README.md"; however, both use the custom *GitHub Flavoured Markdown* (GFM) variant of the Markdown format, and as such, some links and extra visibility elements may not function correctly.

---

## What is Git?

> "*Git is a free and open source distributed version control system designed to handle everything from small to very large projects with speed and efficiency.*"
\- git-scm.com

*Git* is an open-source version control system that allows users to work independently or collaboratively on a project across multiple devices while maintaining changes made to a project between devices and users.

Think of it like a cloud storage service like Microsoft's *OneDrive*, allowing you to upload and download changes. The key difference is that git offers a much wider range of flexibility and control over which files are uploaded, the way files are uploaded, as well as providing many usages and ways of modifying files for different projects.

### How does Git Work?

Git works by creating a *hidden folder* in the root directory of your project, which keeps track of all files and subsequently each change made to those files locally.

<details>
<summary><b> How do I view the hidden .git folder? </b></summary>

> * This folder can be viewed on Windows systems by navigating to the root directory of the project in the File Explorer, then selecting "View" → "Show" and selecting "Hidden Items" to toggle the visibility of hidden files.
> * This folder can be viewed on macOS systems by navigating to the root directory of the project in the Finder and executing *Command + Shift + Period* to toggle the visibility of hidden files.
> * This folder can be viewed on Linux systems in the GUI by navigating to the root directory of the project in the File Manager, selecting "Menu" or "View" (depending on distribution), and then selecting "Show Hidden Files"
> * * Alternatively, the `ls -a` terminal command will list all files in a directory, including hidden files

* A greyed out folder titled ".git" should appear in the root directory using any of the three methods listed above, or in the command output if using the terminal.

</details>

The same git project may be cloned in multiple places or by multiple users, each with unique changes, however these changes will remain locally on the machine until they are uploaded or "*pushed*" to the remote *repository*, in which case the changes can be added to or "*merged*" into the main file structure, overwriting any existing files with updated versions containing uploaded changes. These new changes will then become available to all other copies of the project by downloading or "*pulling*" the new changes.

To learn more about git, see the [Git Documentation](https://git-scm.com/doc), or to learn more about the inner workings of git specifically, see [*Git Internals - Plumbing and Porcelain*](https://git-scm.com/book/en/v2/Git-Internals-Plumbing-and-Porcelain).

### What is a Repository?

A *repository* or *repo* is the container for everything within a project. Think of it like a top-level folder containing every file within the directory, including all files within subfolders. When downloading or "cloning" a git repository, all files be contained entirely within its own folder by default, so this is the easiest way to understand it.

Each repository has a unique *pointer*, *name*, and *domain*, meaning you cannot host multiple repositories with the exact same URL. However, a single repository can be cloned multiple times on the same machine into separate folders, and will be treated as separate instances with their own changes.

The two repository hosting websites you are most likely to use are **GitHub** (<https://github.com/>) and **GitLab** (<https://gitlab.com/>); more specifically, the University of Canterbury GitLab host: **EngGit** (<https://eng-git.canterbury.ac.nz/>).

<details>
<summary><b> Why do I need a Repository Host? </b></summary>

> These websites not only provide a convenient place to upload and download repositories to and from multiple devices, but also provide an easy way to view and manage a repository's file structure, change history, and settings without having to use the terminal or create your own system, as well as easily collaborate with other people on the same project.

</details>

As such, this document covers steps for practices applicable to both hosts; however, most steps should be transferable to any repository host or any use of the terminal.

---

## Pre-Setup

### Install Git

Like any software, Git needs to be installed locally to any machine before being able to use it. Git can be downloaded and run on any Windows, macOS, or Linux device, and can be [downloaded here](https://git-scm.com/downloads).

> This will already be installed on University computers.

### Install Visual Studio Code

The second piece of software that is necessary for following through this document is *Visual Studio Code* (*VS Code* or *VSC* for short). VSC is an open source *Integrated Development Environment* (IDE) which has support for and integrations with many different languages and applications, as well as a large collection of user-made extensions. As such, it provides a convenient place to manage both code-related projects and git integration in a single application. Visual Studio Code can be [downloaded here](https://code.visualstudio.com/Download), and provides a helpful page on [Documentation](https://code.visualstudio.com/docs) to get you started.

> This will already be installed on University computers.

<details>
<summary><b> Why am I using Visual Studio Code? </b></summary>

> There are many other fantastic tools for managing both code projects and git repositories. I find that VSCode provides the best experience and the most user flexibility; however, other IDEs such as [*Visual Studio Community*](https://visualstudio.microsoft.com/downloads/) or [*JetBrains Rider*](https://www.jetbrains.com/rider/download/) provide similar functionality. *VSCode* and *Rider* also have community extensions that allow you to extend and customise your experience.
> > If you have a preferred IDE that you're already familiar with, feel free to use that instead, and follow along where possible.
> 
> * Most IDEs will have similar if not identical implementations of the features outlined in this document, and as such, the knowledge should be mostly transferable; *however*, the layout, steps, and UI may not be the same as described in the VSCode instructions, and may require some experimentation.
> * Terminal commands will function the same *regardless* of the environment they are run in, so there should be no issues following the terminal instructions in different IDEs.
>
> There is also the standalone [*GitHub Desktop*](https://desktop.github.com/download/) application for a more streamlined and hands-off approach to git (however, there isn't as much flexibility in some places; a tradeoff to make the introduction to git less technical). 
> > The terminal can be accessed within GitHub Desktop just like any other IDE by selecting "Repository" → "Open in Command Prompt / Git Bash" from the toolbar, or by executing `Control + ~` | `Command + ~`

</details>

### Add a .gitignore

Before doing anything involving git, ensure that a .gitignore file is placed in the directory of the Unity or Unreal project that you wish to upload.
> This will stop unnecessary files from being pushed to your repository, and is essential for the following instructions in this document.

* **Unity:** [*Unity* gitignore Template](https://github.com/github/gitignore/blob/main/Unity.gitignore)
* **Unreal Engine:** [*Unreal Engine* gitignore Template](https://github.com/github/gitignore/blob/main/UnrealEngine.gitignore)

Gitignore templates for other project structures can be found below. Simply look for the file that has the structure **[your project language/software].gitignore**

> [*List of gitignore template files*](https://github.com/github/gitignore/tree/main)

---

## EngGit Setup Instructions

### Creating a new Repository on EngGit using Visual Studio Code

**Step 1)** Open the Unity Project in Visual Studio Code. There are two ways to do this:

1. In Visual Studio Code: File → Open Folder → (Directory of the project) → Open.
2. In *Unity*, go to File → "Preferences" and ensure that under "External Tools", Visual Studio Code is selected as your External Script Editor. Next, right-click in the Assets window and click "Open C# Project". This will open the entire project in Visual Studio Code and prompt you to install the required [*C# Extension*](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [*Unity Extension*](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc).

<details>
<summary><b> What about Unreal Engine? </b></summary>

*Unreal Engine* uses Visual Studio Community by default. However, if you wish to change the default IDE to Visual Studio Code, you can follow the instructions linked below. *Using Visual Studio Community for code editing and Visual Studio Code for Version Control will **not** cause any problems in your workflow.*
* Unreal Engine also allows you to connect the project to git directly, automatically staging changed files and allowing you to push from within the editor. You can do this by selecting the "<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> *Revision Control*" button on the task bar in the bottom left corner of the project window, and selecting "*Connect to Revision Control*" from the list. In the dropdown of providers, select "*Git (Beta Version)*", then select the blue button labelled "*Accept Settings*".
* > If there is no repository initiatlised in the project directory, a new window will appear, allowing you to enter details and create one.
* Once revision control has been connected, the source control icon <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> will have a green checkmark. Hovering over the button will display information about the repository. CLicking the button again will allow you to view and submit (push) changes without leaving the editor window.

> [Setting Up VS Code for Unreal Engine — Epic Games Documentation](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-visual-studio-code-for-unreal-engine)

</details>

**Step 2)** In Visual Studio Code, along the top toolbar, click "Terminal" → "New Terminal".

> This should ideally be a PowerShell or (Git)Bash terminal to avoid platform-specific syntax errors and inconsistencies.

**Step 3)** Initialise the Repository by running the following terminal commands line by line:

* Make sure you are within the desired folder before initialising the repository, otherwise it will create in the root of the user directory*

<details>
<summary><b> Can I initialize without using the terminal? </b></summary>

> While you can initialize a repository in the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>), this only executes the first command line from the following block, and you get more control over specific details by creating it from the command line.

</details>

```bash
git init --initial-branch=main
git config user.name "YOUR-FIRST-NAME"
git config user.email "YOUR-USER-CODE@uclive.ac.nz"
git add .gitignore
git add README.md
git commit -m "First Commit - Added gitignore and README"
git commit -m "Added gitignore"
git push --set-upstream https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME main
```

* *If the EngGit window appears, enter your credentials and sign in.*

**Step 4)** Enter the following command line:

```bash
git remote add origin https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME.git
```

* This line will be part of the output from the following line after signing in to EngGit, formatted `"/YOUR-REPO-NAME.git"`

> You can also create a repository directly on the EngGit website first and push to the repository's URL, just as in the steps for GitHub outlined below.

**Step 5)** Enter the following command line:

```bash
git push -u origin main
```

* This line will ensure that your project is correctly uploaded/published and pushed to the remote correctly on creation

> You can also open the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) and select the big blue button labelled "*Publish Branch*" to achieve the same thing.

---

#### *Prefilled Terminal Lines for EngGit:*

<details>
<summary><b> Show Code Block </b></summary>

```bash
git init --initial-branch=main
git config user.name "YOUR-FIRST-NAME"
git config user.email "YOUR-USER-CODE@uclive.ac.nz"
git add .gitignore
git add README.md
git commit -m "First Commit - Added gitignore and README"
git push --set-upstream https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME main
git remote add origin https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME.git
git push -u origin main
```

</details>

---

## GitHub Setup Instructions

### Creating a new Repository on GitHub using Visual Studio Code

**Step 1)** Create a new repository on [GitHub.com](https://github.com/)

**Step 2)** Open the Unity Project in Visual Studio Code. There are two ways to do this:

1. In Visual Studio Code: File → Open Folder → (Directory of the project) → Open.
2. In *Unity*, go to File → "Preferences" and ensure that under "External Tools", Visual Studio Code is selected as your External Script Editor. Next, right-click in the Assets window and click "Open C# Project". This will open the entire project in Visual Studio Code and prompt you to install the required [*C# Extension*](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [*Unity Extension*](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc).

<details>
<summary><b> What about Unreal Engine? </b></summary>

*Unreal Engine* uses Visual Studio Community by default. However, if you wish to change the default IDE to Visual Studio Code, you can follow the instructions linked below. *Using Visual Studio Community for code editing and Visual Studio Code for Version Control will **not** cause any problems in your workflow.*
* Unreal Engine also allows you to connect the project to git directly, automatically staging changed files and allowing you to push from within the editor. You can do this by selecting the "<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> *Revision Control*" button on the task bar in the bottom left corner of the project window, and selecting "*Connect to Revision Control*" from the list. In the dropdown of providers, select "*Git (Beta Version)*", then select the blue button labelled "*Accept Settings*".
* > If there is no repository initiatlised in the project directory, a new window will appear, allowing you to enter details and create one.
* Once revision control has been connected, the source control icon <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> will have a green checkmark. Hovering over the button will display information about the repository. CLicking the button again will allow you to view and submit (push) changes without leaving the editor window.

> [Setting Up VS Code for Unreal Engine — Epic Games Documentation](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-visual-studio-code-for-unreal-engine)

</details>

**Step 3)** In Visual Studio Code, along the top toolbar, click "Terminal" -> "New Terminal" to open a new terminal.

> This should ideally be a PowerShell or (Git)Bash terminal to avoid platform-specific syntax errors and inconsistencies.

**Step 4)** Initialize the Repository by running the following terminal commands line by line:

* Make sure you are within the desired folder before initialising the repository, otherwise it will create in the root of the user directory

<details>
<summary><b> Can I initialize without using the terminal? </b></summary>

> While you can initialize a repository in the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>), this only executes the first command line from the following block, and you get more control over specific details by creating it from the command line.

</details>

```bash
git init --initial-branch=main
git config user.name "YOUR-FIRST-NAME"
git add .gitignore
git add README.md
git commit -m "First Commit - Added gitignore and README"
git branch -M main
git remote add origin https://github.com/YOUR-GIT-NAME/YOUR-REPO-NAME.git
```

* *The URL for* `git remote add origin` *can be found at the top of a new blank git repo on* [GitHub.com](https://github.com/)

* *If the Git Credentials Manager window appears, enter your credentials and sign in.*

**Step 5)** Enter the following command line:

```bash
git remote -v
```

* `git remote -v` *will check that the repo is correct before pushing*

**Step 6)** Enter the following command line:

```bash
git push -u origin main
```

* This line will ensure that your project is correctly uploaded/published and pushed to the remote correctly on creation

> You can also open the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) and select the blue button labelled "*Publish Branch*" to achieve the same thing.

---

#### *Prefilled Terminal Lines for GitHub:*

> **Create the Repository on <https://github.com/dashboard> first**

<details>
<summary><b> Show Code Block </b></summary>

```bash
git init --initial-branch=main
git config user.name "YOUR-FIRST-NAME"
git add .gitignore
git add README.md
git commit -m "First Commit - Added gitignore and README"
git branch -M main
git remote add origin https://github.com/YOUR-GIT-NAME/YOUR-REPO-NAME.git
git remote -v
git push -u origin main
```

</details>

---

## Preparing Visual Studio Code for Use with Unity

***Ensure the C# Extension is installed in VSC if used for Unity***

* VSC will automatically prompt you to install the required extension when opening a Unity Project through VSC

* If you need to install the extensions manually: On the left-hand taskbar, select the *Extensions* icon (four squares) to open the Extensions window. Search for "C#" and "Unity", and install the verified extensions published by *Microsoft*
* If you are having difficulties locating the correct extensions, the [*C# Extension*](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [*Unity Extension*](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc) can be downloaded online by following these links

**In the extension settings, ensure that "Omnisharp: Use Modern Net" is *DISABLED***

* This should be disabled by default, and you should be prompted whether you want to enable it when opening a project
* If it is enabled for some reason, to disable it, go to the extension panel on the left-hand taskbar, search for and select the "*C#*" extension to open it in the main window. Select the cog icon labelled "Manage" from along the top, then select *Settings* in the dropdown menu. In the top search bar, search for "omnisharp" and ensure that the "Dotnet > Server: Use Omnisharp" is ***UNCHECKED***

---

## Cloning (Downloading) a Repository

### Locating the Repository URL

To clone your repository, you need to know the URL for your project.

This will be in the format:

*<https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME>* for **EngGit**

***or***

*<https://github.com/YOUR-USERNAME/YOUR-REPO-NAME>* for **GitHub**.


> This can be found on the **EngGit** website by opening a project, clicking the dropdown arrow on the large blue "Code" button, and copying the "Clone with HTTPS" link.
>
> This can be found on the **GitHub** website by opening a project, clicking the dropdown arrow on the large green "Code" button, and copying the URL under the "HTTPS" header.
> > This URL can also be found in the address or URL bar in a browser when viewing a project.
>

### Cloning Using the Visual Studio Code UI

**Step 1)** In Visual Studio Code, open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>), and select "Clone Repository".

**Step 2)** Enter the URL for your project in the window when prompted.

**Step 3)** Choose a file location to clone the repository.
> On University computers, this should be **C:/Local/YOUR-USER-CODE/** or **D:/Local/YOUR-USER-CODE** if available.

### Cloning Using the Terminal

**Step 1)** In Visual Studio Code, along the top toolbar, click "Terminal" -> "New Terminal".

> This should ideally be a PowerShell or (Git)Bash terminal to avoid platform-specific syntax errors and inconsistencies.

**Step 2)** Enter the following line into the terminal with the URL for your project.

```bash
git clone <Your Repo URL>
```

**Step 3)** Choose a file location to clone the repository.
> On University computers, this should be **C:/Local/YOUR-USER-CODE/** or **D:/Local/YOUR-USER-CODE** if available.

---

## Viewing Uncommitted Git Changes

Any uncommitted modification, addition, or deletion to a file within the git repository can be visually assessed in both the Visual Studio Code UI and the terminal, through different methods.

### Viewing Git Changes using the Visual Studio Code UI

Open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>). All modifications to files within the working directory will be listed under the heading "*Changes*".

* A <span style="color:green">Green "U"</span> next to the file means it has been newly added to the repository in this commit
* An <span style="color:orange">Orange "M"</span> next to the file means it has been modified in this commit
* A <span style="color:red">Red "D"</span> next to the file and ~~strikethrough~~ in the filename means it has been deleted in this commit.

> These changes will also be shown within the main File Explorer tab (top of the left-hand taskbar, file icon) within all of the other files, where the entire filename adopts the colours above.
>
> * A <span style="background-color: grey; color: white; padding: 2px; border-radius: 2px">white</span> filename means it has not been modified.

*All files will revert to their <span style="background-color: grey; color: white; padding: 2px; border-radius: 2px">white</span> filenames after the changes have been committed and pushed.*

### Viewing Git Changes using the Terminal

To view the current status of files within the working repository using the terminal, run the following command line:

```bash
git status
```

This will list information about the state of the current git directory, including listing *untracked files* (additions), *modifications* (changes), *deletions*, and whether it is up to date with the remote.

---

## Staging and Pushing (Uploading) Changes

To integrate a change to the project and allow it to be accessed elsewhere, the change must be *staged* (prepared) and *pushed* (uploaded) to the remote repository. This will allow you to sync your work across devices or with other members of your team.

***YOU SHOULD ALWAYS PULL CHANGES BEFORE PUSHING IF YOU CAN HELP IT***

### Pushing Changes Using the Visual Studio Code UI

After making changes, open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>)

To prepare files for upload, click the small **+** icon next to the word "*Changes*" to stage these changes for upload.

> This icon will only appear by hovering over the word "*Changes*"
>
> You can also stage specific files by hovering over a changed file, right-clicking it, and selecting "*Stage Changes*" from the context menu.

Once the files are under the heading "*Staged Changes*", enter a message into the window above detailing your changes, and click "*Commit*".

* You may also need to repeat this message and click "*Publish*".

### Pushing Changes Using the Terminal

Before you can push changes, you must *stage* them, or prepare them for upload

To **stage** a specific changed file using the terminal,, run the following terminal commands.

```bash
git add [Filename]
```

To stage **all** files using the terminal, run one of the following command lines:

```bash
git add .
```

or

```bash
git add --a
```

* This will stage **all** the changed files detected in the local repository

To add a **commit message** to the push using the terminal, run the following command line:

```bash
git commit -m "[Commit Message]"
```

> **Make sure to include the quotation marks around the commit message; otherwise, the command will not enter correctly.**

To **push** all staged changes to the remote repository using the terminal, run the following command line:

```bash
git push
```

This will push all changes to the default URL and branch. We set these earlier in the setup process, and by default, they should be *origin* and *main* respectively.

> To specify a specific remote or branch to push to, instead use `git push <Remote> <Branch>`

---

## Un-Staging Changes

Sometimes you may stage changes but need to unstage them to edit them or stop them from being pushed.

### Un-Staging Changes Using the Visual Studio Code UI

Unstaging changes can be done the same way as staging changes. Open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>)

To unstage changes, click the small **-** icon next to the word "*Staged Changes*".

> This icon will only appear by hovering over the words "*Staged Changes*"

### Un-Staging Changes Using the Terminal

You can run the following terminal command to unstage all changes.

```bash
git restore --staged .
```

* This command can be especially useful when the VSC UI is unresponsive.

---

## Discarding Changes

Discarding changes will reset all unpushed changes made to files in the repository back to the state they were in at the time of the last push. This can be useful when scrapping large numbers of changes or when you need a clean repository.

### Discarding Changes Using the Visual Studio Code UI

To discard a specific changed file, first open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) to view your changed files.

Hover over a changed file, right-click it, and select "*Discard Changes*" from the context menu. In the pop-up window, select the button labelled "*Discard File*" to confirm the discard.

To discard **all** changes, hover over the word "*Changes*", right-click it, and select "*Discard All Changes*" from the context menu. In the pop-up window, select the button labelled "*Discard File*" to confirm the discard.

This method may not always work, in which case you should try:

### Discarding Changes Using the Terminal

To discard a specific changed file, run one of the following terminal commands.

```bash
git checkout [Filename]
```

or

```bash
git restore [Filename]
```

To discard **all** changes and reset the working directory, run one of the following terminal commands.

```bash
git checkout -- .
```

or

```bash
git restore -- .
```

---

## Stashing and Popping Changes

Stashing a change allows you to take all changes made to files and store them for later, allowing you to pull changes and possibly avoid a merge conflict. Stashed changes are stored until they are popped (restored), updated, or discarded.

### Stashing Changes Using the Visual Studio Code UI

To stash a specific changed file, first open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) to view your changed files.

Hover over a changed file, right-click it, and select "*Stash Changes*" from the context menu. A text field will appear, prompting you to add a stash message. Enter a message, then press Enter to stash the change. The change made will be put into the stash, and the file will revert to its state at the last commit.

To stash **all** changes, hover over the word "*Changes*", right-click it, and select "*Stash All Changes*" from the context menu. A text field will appear, prompting you to add a stash message. Enter a message, then press "Enter" to stash the changes. All changes made will be put into the stash, and all files will revert to their state at the last commit.

### Popping Changes Using the Visual Studio Code UI

To pop (restore) a change, hover over the heading **CHANGES** at the top of the source control window. Select the **...** that appears on the right side of the heading. From the context menu, select "Stash" > "Pop Stash". This will bring up a list of known stashes that you can select to restore.

### Stashing Changes Using the Terminal

To stash all changed files in the working directory, run the following terminal command:

```bash
git stash
```

This will create a stash with the most recent commit message. To provide more detail to your stash, run the following terminal command:

```bash
git stash save "[Stash Message]"
```

To view all stashes, run the following terminal command:

```bash
git stash list
```

### Popping Changes Using the Terminal

To restore all stashed changes, run the following terminal command:

```bash
git stash pop
```

By default, `git stash pop` will pop the most recently made stash. To specify a specific stash to pop, run the following terminal command:

```bash
git stash pop stash@{[Stash Index]}
```

* i.e. `git stash pop stash@{2}` will restore the second most recent stash

> More information can be found on the [*Git Stash* section of the Git Documentation](https://git-scm.com/docs/git-stash), or this Atlassian [help page on Git Stashing](https://www.atlassian.com/git/tutorials/saving-changes/git-stash).
---

## Pulling Changes

To pull changes from the remote repository, you must first have the project or folder open within VSC. To do this, along the top toolbar, select "File" → "Open Folder", then navigate to your cloned repository and open the folder.

***YOU SHOULD ALWAYS PULL CHANGES BEFORE PUSHING IF YOU CAN HELP IT***

### Pulling Using the Visual Studio Code UI

In Visual Studio Code, open the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>), and press the "Sync Changes" button to download all uploaded changes.

### Pulling Using the Terminal

In Visual Studio Code, along the top toolbar, click "Terminal" → "New Terminal" to open a new terminal.

> This should ideally be a PowerShell or (Git)Bash terminal to avoid platform-specific syntax errors and inconsistencies.

To pull changes to a local repository using the terminal, run the following command line:

```bash
git pull
```

---

## Dealing with Clashes and Merge Conflicts

Sometimes, when the same file has been worked on across devices or users without pulling local changes before attempting to push, you may run into ***Merge Conflicts*** where there exist two versions of the same file within git. When this occurs, the repository doesn't know which one to use. The two files exist in *superposition* of two states at once until resolved.

Merge conflicts will appear visually in the Source Control tab on the left-hand taskbar (Branch icon, <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) under a new heading labelled "*Merge Changes*" with a red "<b style="color:red">!</b>" next to the file name, as well as a number indicating the amount of conflicts within the file.

> You can also view merge conflicts using the terminal by running `git status`

Sometimes merge conflicts can be easily resolved by simply opting to keep the incoming or local version of a file, or even sometimes manually merging the two versions of a file, for example, going through and accepting lines of code from both versions of the changed file to create a new, third file that is an amalgamation of the two prior versions.

Sometimes, however, merge conflicts can be much harder to fix, and can be a serious cause of headaches when multiple merge conflicts appear in your repository at once.

The best way to deal with merge conflicts, of course, is to *avoid them altogether*. This is why it's important to pull remote changes before uploading new local changes, to ensure that you aren't overwriting a file locally that someone else has already updated remotely. Always do your best to sync the most recent changes to a file before editing it yourself.

* When working in a group, it is especially important to follow this and to ensure that multiple people aren't working on the same file at the same time. This could lead to serious loss of work for one or both parties. Files beyond those that can be easily changed in a text editor are much harder to resolve through a merge without some loss of data.

Merge conflicts can be resolved using both the Visual Studio Code UI and the terminal; however, the UI gives a much better visual representation of what files are conflicting, as well as viewing possible errors that could come as a result of merging. This provides ***much easier*** control over which parts of a file to accept in a merge. As such, it is highly recommended to resolve merge conflicts this way.

> To reset or restart a merge, you can execute `Control + Shift + Period` | `Command + Shift + Period` or select the select the search bar at the top of the window and type `> "git abort merge"` to reset your directory back to the state it was in before the merge was started.

### Dealing with Merge Conflicts using the Visual Studio Code UI

Visual Studio Code has two methods of resolving merge conflicts: the *Inline Merge Editor* or the *Three-Way Merge Editor*.

#### In-Line Editor Method

The *Inline Merge Editor* will open the affected file and highlight differences within the file in either <span style="background-color:aquamarine; color:dimgrey; padding: 2px; border-radius: 2px"> green (Aquamarine)</span> for local changes - labelled "*(Current Change)*", or <span style="background-color:cornflowerblue; color:white; padding: 2px; border-radius: 2px">blue (Cornflower Blue)</span> for incoming changes - labelled "*(Incoming Change)*".

Merge conflicts within a file can be viewed throughout a file easily by looking at the scrollbar on the right of the window. Blue and green highlights will appear wherever there is a merge conflict within a file.

Four text options will appear above the highlighted text:

* **Accept Current Change** will overwrite the incoming change with the local change, removing the highlight and inserting the local changes back into the file
* **Accept Incoming Change** will overwrite the local change with the incoming change, removing the highlight and inserting the remote changes into the file in place of the current changes
* **Accept Both Changes** will merge both modified sections if possible, removing the highlight and inserting both versions of the section into the file
* **Compare Changes** will open both the local and remote versions of the file side-by-side to more easily compare changes on the same line. Removed lines will appear highlighted in <span style="background-color:red; color:white; padding: 1px; border-radius: 2px">red</span>, whereas added lines will appear highlighted in <span style="background-color:green; color:white; padding: 2px; border-radius: 2px">green</span>.

***Once all conflicts have been resolved, you must stage the affected files and commit them as a new push.
The commit message will be autofilled in the format `"Merge Branch '[affected branch]' into '[current branch]'`***

#### Three-Way Editor Method

The *Three-Way Merge Editor* can be accessed by selecting the blue button labelled "*Resolve In Merge Editor*" in the bottom right corner of an open file with merge conflicts.

Merge conflicts within a file can be viewed in the lower window. In the top right of the upper taskbar, a grey button labelled "[*x*] *Conflict(s) Remaining*" will be present listing the number of conflicts within the file. Clicking this button will take you to the next conflict present in the file.

This will open three separate versions of the file:

* The **top left** will display the version of the file with **incoming changes**
* The **top right** will display the version of the file with **local changes**
* The **bottom** window will display the **result** of the merge

The top left and top right windows have the conflicting lines highlighted in <span style="background-color:olivedrab; color: white; padding: 2px; border-radius: 2px">green (Olive Drab)</span> in their respective files with three buttons above the affected line.

The **top left** (incoming changes) and **top right** (local changes) windows have the following buttons:

* **Accept Incoming / Current** will select the incoming/current change and place it into the lower window, overwriting the change in the opposite window
* **Accept Combination** will combine the two changed lines into a single line and place the result into the lower window
* **Ignore** will ignore one or both of the changes present, allowing you to mark the merge as complete by removing the affected lines.

The **bottom** (result) window will have a label indicating where the resulting line came (*Incoming, Current, Combination*) from and the following buttons ***only after*** selecting a conflicted line to be inserted:

* **Remove [Incoming / Current]** will remove the changed line from its source file from the result, allowing you to select another line to insert or remove part of a combined line
* **Manual Resolution** will allow you to manually type or edit a line to resolve the conflict
* **Reset to Base** will remove any lines injected into the result file

Once you have resolved all conflicts in a file and are happy with the state of the result file, you can integrate and commit the changes and reintegrate the file. In the lower window, you can select the blue button labelled "*Complete Merge*" in the bottom right corner of the lower window to save the changes.

***Once all conflicts have been resolved, you must stage the affected files and commit them as a new push.***

***The commit message will be autofilled in the format `"Merge Branch '[affected branch]' into [current branch]`***

---

## Adding Collaborators to a Repository

### Adding Collaborators On EngGit

On the EngGit website, open your project. On the left-hand panel, hover over "Manage" and select "Members", and in the popup menu, enter the person's user code or name. Once the account has been found, you can select their *Role* in the project. Click "Add [*name*]" to invite them to the project.
> If you intend to have them contribute to the project, select the *Developer* Role.
> You can also provide an expiry date for their access to the project. Unless specified otherwise, leave this blank.

### Adding Collaborators On GitHub

On the GitHub website, open your project. Along the top bar, click the "Settings" button, then along the left-hand panel, under the "Access" heading, select "Collaborators".

> Here, you can manage the visibility of your repository (Public / Private) to allow anyone to view the project, or invite specific accounts to grant them access.

Select "Add People", then enter their username, name, or email in the pop-up menu. Once the account has been found, select the desired person and click "Add [*name*]" to invite them to the project.

---

## Changing a Git Commit Message

*Ensure you have the correct git repository open before attempting to change a commit message.*

To change a recently committed git commit message, run the following command line:

```bash
git commit --amend -m "[New Commit Message]"
```

* This will only work if the commit has only been committed locally and has not been pushed to the remote repo.

If you have already pushed the changes to the remote repository, you will need to run a second command line:

* ***This command rewrites commit history and could be incredibly destructive, especially in a group setting, as other group members will need to re-clone or manually fix their local history.***
* ***Only use this if you are working alone or you know what you're doing***

```bash
git commit --amend -m "[New Commit Message]"
git push --force-with-lease origin
```

---

## Git Branches

A branch is like a secondary, parallel root folder of your repository that allows you to work on elements of a project without interfering with the main project. It is often used for feature testing before integration by "*forking*" (copying) from the main branch to get an exact copy of the project to test in before merging your changes back in. Another use for branches is having multiple distinct projects under one repository, such as having a separate branch for each week of a lab to avoid having a unique repository for each lab.

### Creating New Branches

You can create and use branches by either:

* Creating the branch on the local remote first and pushing it to the host, or
* creating the branch on the hosting website first and pulling it to the local remote

Both methods function the same way and achieve the same outcome.

#### Local Remote Branch Creation

##### Creating a New Branch through the Visual Studio Code UI

In Visual Studio Code, along the bottom, select the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". This will bring up the list of git branches.

To create a new branch, select the first option *"**+** Create new branch..."*.

Enter a name for the new branch in the field provided. This will create and switch into a new branch.

> You can also execute `Control + Shift + Period` | `Command + Shift + Period` or select the search bar at the top of the window and type `> "git create branch"`, then enter a branch name to create a branch

Open the Source Control tab on the left-hand taskbar by selecting the Branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) and select the blue button labelled "*Publish Branch*" to push the new branch to the remote repository.

##### Creating a New Branch through the Terminal

In Visual Studio Code, along the top toolbar, click "Terminal" → "New Terminal" to open a new terminal.

> This should ideally be a PowerShell or (Git)Bash terminal to avoid platform-specific syntax errors and inconsistencies.

To create a new branch using the terminal, run the following command lines:

```bash
git checkout -b <Branch Name>
git branch
git push -u origin <Branch Name>
git branch -a
```

This will:

* Create a new branch,
* Switch into the new branch,
* List the current local branches to verify that we are on the correct branch,
* Push and Upload/Publish the newly created branch to the remote repository, and
* List all remote branches to verify that the new branch has been pushed

#### Host Website Branch Creation

##### Creating a New Branch on EngGit

On the **EngGit** website, open your project.

To **view your existing branches** or switch to a different branch, select the button underneath the project icon and name with the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". This will list all of your current branches, as well as the default branch. Select any of the listed branches to switch into it.

To **add a new branch**, locate the **+** button underneath the project icon and name, and select "*New branch*" from the dropdown.

* Enter a name for the new branch in the field provided, and select an existing branch to create from.
* * *If you are unsure, create the new branch from "*main*".*
* Select the blue button labelled "Create branch" to add a new branch to your repository. You will automatically be placed back into the project view inside the new branch

##### Creating a new Branch on GitHub

On the **GitHub** website, open your project.

To **view your existing branches** or switch to a different branch, select the button underneath the project icon and name with the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". This will list all of your current branches, as well as the default branch. Select any of the listed branches to switch into it.

To **add a new branch**, select the button underneath the project icon and name with the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". At the bottom of the list of branches, select "*View all branches*". Alternatively, select the link next to the dropdown labelled " <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> [x] Branches". This will open the branch manager. In the new window, select the green button in the upper right corner labelled *New branch*

* Enter a name for the new branch in the field provided, and select an existing branch to create from.
* * *If you are unsure, create the new branch from "*main*".*
* Select the green button labelled "Create branch" to add a new branch to your repository. You will automatically be shown a list of all branches, and can now navigate back to the project root and switch branches.

### Checking for Newly Created branches

To update the local list of branches or check for new remote branches, run the following command line:

```bash
git remote update origin --prune
```

To list all local and remote branches that Git knows about, run the following command line:

```bash
git branch -a
```

### Switching Branches

#### Switching Branches using the Visual Studio Code UI

In Visual Studio Code, along the bottom taskbar, select the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". This will bring up the list of git branches. Select any one of the listed branches to switch into it.

#### Switching Branches using the Terminal

To switch branches, run one of the following command lines:

```bash
git checkout <Branch Name>
```

or

```bash
git switch <Branch Name>
```

### Renaming Branches

If you want to rename a branch, there are slightly different methods than must be taken depending on whether the branch is local only, or whether it has been pushed to a remote already.

> It's best to ensure you name branches correctly the first time to avoid having to rename them. Check the name before publishing the branch so that in case you do need to rename it, you only need to change the local name.

#### Renaming Branches using the Visual Studio Code UI

To rename a branch, execute `Control + Shift + Period` | `Command + Shift + Period` or select the search bar at the top of the window and type `> "git rename branch"`, then enter a branch name to rename the current branch.

This will rename the current branch to the new entered name, however it will only rename it locally. To rename it on the remote, you will need to execute some terminal commands, which are listed in the following section.

#### Renaming Branches using the Terminal

##### Renaming a Local Branch

To rename a **local branch** (one that hasn't been pushed yet), run the following terminal command:

```bash
git branch -m <Old Branch Name> <New Branch Name>
```

This allows you to rename the branch in one line without needing to switch into it first.

* *If this fails*, separate the command out into two lines by first running `git checkout <Old Branch Name>` to switch into the branch, then run `git branch -m <New Branch Name>”` to rename it.

##### Renaming a Remote Branch

To rename a **remote branch** (one that ***has*** been pushed) such that both the local and remotes are updated, you need to run some extra command lines:

> It may help to run `git branch -a` to view the names of local and remote branches to ensure that branches are being named correctly

**Step 1)** First, run the following commands to change the local remote name as above:

```bash
git branch -m <Old Branch Name> <New Branch Name>
```

**Step 2)** Once the branch has been renamed, delete the branch with the old name from the remote:

```bash
git push origin --delete <Old Branch Name>
```

**Step 3)** Next, push the branch with the new name to the remote in place of the old one using:

```bash
git push origin -u <New Branch Name>
```

**Step 4)** Finally, refresh the list of branches by running:

```bash
git remote update origin --prune
```

This will update a branch name both locally and when viewed on the remote repository host, and will maintain the new branch name wherever the repository is cloned.

### Deleting Branches

#### Deleting Branches Locally

##### Deleting Branches using the Visual Studio Code UI

To delete a branch, execute `Control + Shift + Period` | `Command + Shift + Period` or select the search bar at the top of the window and type `> "git delete branch"`, then select a branch to delete from the list

> This list will ***not*** show the currently selected branch. If the desired branch isn't showing, switch back to main and try again

This will delete the desired branch, however it will only delete it locally. To delete it on the remote, you will either need to execute some terminal commands or delete it on the host website. Both methods are listed in the following sections.

##### Deleting Branches using the Terminal

To delete a **local branch** (one that hasn't been pushed yet), run the following terminal command:

```bash
git branch -d <Branch Name>
```

To delete a **remote branch** (one that ***has*** been pushed), run the following terminal command:

```bash
git push -d origin <Branch Name>
```

#### Deleting Branches on a Host Website

##### Deleting Branches on EngGit

On the **EngGit** website, open your project.

On the right-hand side of the window, under the heading *Project information*, select the link labelled " <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> [x] Branches" to open the branch manager.

From the list, select the three vertical dots to the right of a branch, and from the dropdown select "**Delete Branch**".

Confirm the branches deletion in the popup window that appears by selecting the red button labelled "**Yes, delete branch**".

##### Deleting Branches on GitHub

On the **GitHub** website, open your project.

Select the button underneath the project icon and name with the branch icon (<i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>) labelled "*main*". At the bottom of the list of branches, select "*View all branches*". Alternatively, select the link next to the dropdown labelled " <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i> [x] Branches". This will open the branch manager.

From the list, select the trashcan icon to the right of a branch, to delete the branch.

* If you attempt to delete the main branch, a popup will appear in the bottom left corner stating "*You can't delete the default branch.*"

> There will be no confirmation for the deletion of branches, so **be careful**

---

## Configuring Git Large File Systems

If a file is over 100mb, hosting websites like GitHub may / will refuse to push it normally.

> (This is a limitation with the file host, not with Git itself, and as such may or may not be an issue.)

To fix this, install **GIT LFS** and use it to track and push the large files separately.

> Be aware that sites such as GitHub have a limit on the amount of storage held in LFS without a charge.
***Use wisely.***
>
> * [About Git Large File Storage — GitHub](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-git-large-file-storage)

Enter the following commands line by line:

```bash
git lfs install
git lfs track [Path to Large File]
git add [Path to Large File]
git commit -m "Added large file to Git LFS"
git push
```

The installation will add a new file named `.gitattributes` to the root of your repository, and will hold the filepaths to all files tracked by Git LFS.

---

## Restoring / Uncommiting Committed Changes

If you need to restore or edit files that you have already staged and committed, you can use the following command to pull back the changes and modify them before pushing.

```bash
git reset --soft HEAD~1
```

This will reset the state of git to one commit back, bringing back your changes in a non-destructive way and without forcing you to push them.

---

## Adding a file to the .gitignore

If you want to ignore a file and stop it from being pushed to the remote repository, you can add it to your project's .gitignore

<details>
<summary><b> What if I don't have a .gitignore file? </b></summary>

> If you need a gitignore template, you can find one from the [List of gitignore template files](https://github.com/github/gitignore/tree/main).
>
> You can also create a new blank one by running the terminal command `touch .gitignore` in a **Git(Bash) Terminal**.

</details>
<br>

To ignore a file, you can add its filepath to the .gitignore directly, or use one of the more technical file searching patterns as shown in the example below from the documentation:

```bash
# ignore all .a files
*.a

# but do track lib.a, even though you're ignoring .a files above
!lib.a

# only ignore the TODO file in the current directory, not subdir
/TODO

# ignore all files in any directory named build
build/

# ignore doc/notes.txt, but not doc/server/arch.txt
doc/*.txt

# ignore all .pdf files in the doc/ directory and any of its subdirectories
doc/**/*.pdf
```

If you want to ignore a file that has already been committed but not pushed, you need to untrack the file first by running the following terminal command:

```bash
git rm --cached [Filename]
```

> More information can be found on the [Ignoring Files section of the Git Documentation](https://git-scm.com/book/en/v2/Git-Basics-Recording-Changes-to-the-Repository#_ignoring).

---

## Pushing a Repository to / Hosting on Multiple Remotes

***i.e., Hosting on GitHub and EngGit Simultaneously***

To push an existing repository to a new remote, you can run the following terminal commands.

* *Be very careful when doing the following, as ensuring each line uses the correct URL and name is crucial*

**Step 1)** This will list the current "origin" that we are pushing to:

```bash
git remote get-url origin
```

**Step 2)** This will rename the current origin branch:

```bash
git remote rename origin [Current Origin Host]
```

* For example, if the URL in the first step is a GitHub URL (meaning that the repo is being hosted on GitHub), running `git remote rename origin GitHub` will rename the origin branch to GitHub, indicating where it is being hosted.

**Step 3)** This will add a second repository to the git under the name and URL provided:

```bash
git remote add [New Repository Host] <new repository url>
```

* For example, running  `git remote add EngGit https://eng-git.canterbury.ac.nz/...` will create a new remote named *EngGit*.

**Step 4)** This will add the existing repository as a fetch URL:

```bash
git remote add origin <existing repository url>
```

**Step 5)** This will ensure that whenever pushing to origin, it will push to the push url provided:

```bash
git remote set-url --add --push origin <push url>
```

* This can be run for any number of additional remotes that you wish to push to.

**Step 6)** This will check that multiple remotes have been setup correctly:

```bash
git remote show origin
```

* It should list a single fetch URL which is the original repository, and multiple Push URLs, which have been configured by the user.

**Step 7)** This will ensure that the **entire repository** (including all branches) is pushed to both URLS right after creation to keep them up to date, and ensure that the hierarchy of branches remains the same across hosts:

```bash
git push --all origin
```

### Example: Setting a Remote from EngGit to push to both EngGit and GitHub

<details>
<summary><b> Show Code Block </b></summary>

```bash
git remote get-url origin
git remote rename origin EngGit
git remote add GitHub <GitHub URL>
git remote add origin <EngGit URL>
git remote set-url --add --push origin <EngGit URL>
git remote set-url --add --push origin <GitHub URL>
git push --all origin
```

</details>

### Example: Setting a Remote from GitHub to push to both GitHub and EngGit

<details>
<summary><b> Show Code Block </b></summary>

```bash
git remote get-url origin
git remote rename origin GitHub
git remote add EngGit <EngGit URL>
git remote add origin <GitHub URL>
git remote set-url --add --push origin <GitHub URL>
git remote set-url --add --push origin <EngGit URL>
git push --all origin
```

</details>
<br>

> Adapted from: [*Pushing to multiple git remotes simultaneously — Jeff Kreeftmeijer*](https://jeffkreeftmeijer.com/git-multiple-remotes/#fnr.2)

---

## Migrating a Repository

Migrating a repository will bring everything within the project, including all files, branches, and commit history from one host to another. This allows you to move a repository without losing any work and enables you to continue working seamlessly.

### Moving from EngGit to GitHub

**Step 1)** Go to [GitHub](https://github.com/dashboard) and select the green button labelled "*New project*" on the left-hand panel.

**Step 2)** In the new project window, right underneath the "**Create a new repository**" heading, select the small blue underlined text that says "[*Import a repository.*](https://github.com/new/import)"

> You can also click the link above to go directly to <https://github.com/new/import>

**Step 3)** In the provided fields, enter the **EngGit repository URL** in the field labelled "*The URL for your source repository*", and **your EngGit credentials** in the fields labelled "*Your username for your source repository*" and "*Your access token or password for your source repository*" respectively.

<details>
<summary><b> Why do I need to enter my credentials? </b></summary>

> This is essential as your EngGit account is protected by the University of Canterbury, and as such, you cannot make the repository public; as repositories hosted on EngGit can *only* be accessed by University of Canterbury personnel. If simply importing from GitLab itself, you may not need to do this.

</details>

Under the heading "**Your new repository details**", set the **name of the project** in the field labelled "*Repository name*".

Finally, set the visibility of the project at the bottom of the page, and select the green button labelled "*Begin import*".

Following these steps will start the import process from GitLab to GitHub. This process could take some time, depending on how large the existing repository is.

When it is finished, the page will display a link to your newly imported project directory.

### Moving from GitHub to EngGit

**Step 1)** Make sure the GitHub repository you wish to migrate has its visibility set to *Public*.

* This can be done by going into the GitHub repository settings, scrolling down under the "*General*" tab and selecting "*Change visibility*" from within the **"Danger Zone"**. You can then select "*Change to public/private*", and then confirm this selection in the popup window(s).

**Step 2)** Go to [EngGit](https://eng-git.canterbury.ac.nz/) and select the blue button labelled "*New project*" in the upper right corner.

**Step 3)** Select "Import Project" from the selection.

**Step 4)** Select the button labelled "🔗*Repository by URL*". This will expand the page and provide the necessary options.

* In the provided fields, enter the **GitHub repository URL** in the field labelled "*Git repository URL*", the **name of the project** in the field labelled "*Project name*", and the **end of the URL** that will appear on EngGit in the field labelled "*Project slug*".
* * The name and slug for the new project will likely be automatically filled when entering the URL.
* Finally, set the visibility of the project at the bottom of the page, and select the blue button labelled "*Create project*".

> You can also add an optional description in the field labelled "*Project description (optional)*", or enter access credentials in the "*Username (optional)*" and "*Password (optional)*" if you cannot change the visibility of your repository.

Following these steps will start the import process from GitHub to GitLab. This process could take some time, depending on how large the existing repository is.

When it is finished, you will be loaded into the newly imported project directory.

> You may also want to reset your GitHub visibility after importing has finished.

### Updating URL after migration

To update the URL of a local repository after migrating to/from another git hosting website, you can use the following terminal command:

```bash
git remote set-url origin <New URL>
```

Run `git remote show origin` to verify that the remote has been updated to the new URL provided.

---

## Merging Multiple Separate Repositories into branches under One Single Repository

If you run out of repository space or need to organise your repositories, you may want to merge multiple repositories into separate branches of a single repository.

* For example, if you hosted each week of a lab as a separate repo, you may want to pull each repository into its own branch of a new repository.

To do this, create a new empty repository and run the following terminal commands:

> For this set of commands, it is **very important** to use a PowerShell or Git(Bash) terminal, or else one of the lines will not execute.

```bash
git init --initial-branch=main
git config user.name "NAME"
git config user.email "YOUR-USER-CODE@uclive.ac.nz"
git commit --allow-empty -m "Initial dummy commit"
```

* Switch back to *main* before the next set of steps

```bash
git checkout -b [Lab_X]
git remote add --fetch [Lab_X] <[Lab_X] repo URL>
git merge [Lab_X]/main --allow-unrelated-histories
mkdir [Lab_X]
dir -exclude [Lab_X] | foreach { git mv $_.Name [Lab_X] }
```

If you get the error: `[Rename from 'Content' to '[LabX]/Content' failed. Should I try again? (y/n)]`: Press "*y*"

```bash
git commit -m "Move [Lab_X] files into subdir"
```

* Loop back until all desired repositories have been added

```bash
git push --set-upstream https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME.git
git remote add origin https://eng-git.canterbury.ac.nz/YOUR-USER-CODE/YOUR-REPO-NAME.git
```

* Go through each branch and publish to origin

---

## Useful Links

Git Documentation: <https://git-scm.com/docs/git>

EngGit: <https://eng-git.canterbury.ac.nz/>

GitLab: <https://about.gitlab.com/>

GitLab Git Tutorials <https://docs.gitlab.com/tutorials/learn_git/>

GitHub: <https://github.com/>

GitHub Git Tutorials: <https://docs.github.com/en/get-started/git-basics>

Atlassian Git Tutorials: <https://www.atlassian.com/git/tutorials>

Visual Studio Code Source Control Documentation: <https://code.visualstudio.com/docs/sourcecontrol/overview>

Markdown Syntax Guide: <https://www.markdownguide.org/basic-syntax/>

CSS Preset Colour Names: <https://www.w3schools.com/colors/colors_names.asp>

Source Control Branch Icon <i style="display: inline-block; width: 12px"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path style="fill: currentColor" d="M80 104a24 24 0 1 0 0-48 24 24 0 1 0 0 48zm80-24c0 32.8-19.7 61-48 73.3l0 87.8c18.8-10.9 40.7-17.1 64-17.1l96 0c35.3 0 64-28.7 64-64l0-6.7C307.7 141 288 112.8 288 80c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3l0 6.7c0 70.7-57.3 128-128 128l-96 0c-35.3 0-64 28.7-64 64l0 6.7c28.3 12.3 48 40.5 48 73.3c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-32.8 19.7-61 48-73.3l0-6.7 0-198.7C19.7 141 0 112.8 0 80C0 35.8 35.8 0 80 0s80 35.8 80 80zm232 0a24 24 0 1 0 -48 0 24 24 0 1 0 48 0zM80 456a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg></i>: <https://fontawesome.com/icons/code-branch?s=solid>

---

## Copyright Information

**<a href="https://github.com/SaxySam/Git_Instructions">"*GIT OVERVIEW AND SETUP INSTRUCTIONS*"</a> © 2025 by <a href="https://github.com/SaxySam">Samuel Kennedy</a> is licensed under <a href="https://creativecommons.org/licenses/by-nc-sa/4.0/">CC BY-NC-SA 4.0</a>**

<img src="https://mirrors.creativecommons.org/presskit/icons/cc.svg" alt="" style="max-width: 1em;max-height:1em;margin-left: .2em;"> <img src="https://mirrors.creativecommons.org/presskit/icons/by.svg" alt="" style="max-width: 1em;max-height:1em;margin-left: .2em;"> <img src="https://mirrors.creativecommons.org/presskit/icons/nc.svg" alt="" style="max-width: 1em;max-height:1em;margin-left: .2em;"> <img src="https://mirrors.creativecommons.org/presskit/icons/sa.svg" alt="" style="max-width: 1em;max-height:1em;margin-left: .2em;">

***No AI slop was used in the creation of this document.***

---

[*Back To Top*](#git-overview-and-setup-instructions)
