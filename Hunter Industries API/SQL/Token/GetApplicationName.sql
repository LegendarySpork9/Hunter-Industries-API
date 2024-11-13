select Name from Application with (nolock)
join Authorisation with (nolock) on Application.PhraseID = Authorisation.PhraseID
where Phrase = @Phrase