# The Blazor Roundup issues

One Markdown file per sent issue, named with the issue number in three digits (`001.md`, `002.md`, ...). The site reads every file here at startup, so adding one and pushing to `main` publishes it: the issue page at `/roundup/<number>`, the archive, the home page, the sitemap and the RSS feed all update with that deploy.

Each file starts with front matter:

```markdown
---
number: 3
sent: 2026-10-22
subject: "The subject line as sent"
preview: "The preview text as sent"
---

Hi everyone, Izzy here. ...

## From Enable Dev Mode
...
```

- `number` and `sent` (`yyyy-MM-dd`) are required, and so are `subject` and `preview`. Quote them if they contain a colon.
- The body is the issue as it went out, from the intro to the sign-off. Leave out the editor notes, the subject and preview lines, the Unconfirmed list and the Sources. A leading `# title` is dropped, since the page shows the subject as its title.
- Two lists in a row with only a blank line between them merge into one. Give the second a different bullet (`*` instead of `-`), as the lead story's notes do in the samples.
- Plain Markdown only: headings, paragraphs, lists, links, bold, quotes and code. Raw HTML is turned off.
- A file the site can't read is logged and left out; the rest of the site still starts.

This file isn't an issue: only names made of digits are read.
