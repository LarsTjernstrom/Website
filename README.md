# Website
Playground for creating a Website CMS using Starcounter mapping

## Functionality

Create a website that has 3 main layout regions: `header`, `content` and `footer`.

The `header` region has a video and a list of 2 links: "Home" and "Team". Content of `header` is the same for whole website.

The `footer` region has copyright info. Content of `footer` is the same for whole website.

The `content` region is different for each of the Website pages:

- for `/website/`, it is a welcome message
- for `/website/team`, it is a list of links to the team members ("Konstantin", "Tomek", "Marcin")
- for `/website/team/{{teammember}}`, it is sidebar layout with 2 regions:
  - in the left region there is the list of links to the team members
  - in the right region there is a bio note about the currently selected team member
  
## Goals

1. The `header` video does not interrput when you switch pages (because of page morphing)
2. All links work correctly if you open them in a new tab
3. On a team member page, switching to another team member does not reload any template - only the data

## Future goals

1. Integrate with Images app: Add photos to team members, show advertisement JPG in the sidebar left column
2. Integrate with Products app: Display products list in the sidebar left column and products details in the right column
3. Be able to create new pages through an admin panel
