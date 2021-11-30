# Pass

Cross platform pass client written in .NET.

## Usage

To provide platform independence, *Pass* is using public and private key from
separate files. To obtain the key files from `gpg` you have to export them:

```shell
gpg --armor --export-secret-keys <KEY-ID> > private.asc
gpg --armor --export <KEY-ID> > public.asc
```

*Pass* is looking for these files in `$HOME/documents`.

Further the password store directory with the encrypted files should be
`$HOME/.password-store`.
