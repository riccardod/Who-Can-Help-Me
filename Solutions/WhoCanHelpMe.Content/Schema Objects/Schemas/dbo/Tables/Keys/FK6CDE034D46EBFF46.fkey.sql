﻿ALTER TABLE [dbo].[n2Item]
    ADD CONSTRAINT [FK6CDE034D46EBFF46] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[n2Item] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

