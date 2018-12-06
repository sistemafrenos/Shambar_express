SELECT     dbo.Documentos.Iddocumento, dbo.Documentos.Fecha, dbo.Documentos.Numero, 'CAJERO' AS Expr1, dbo.Terceros.CedulaRif, dbo.Terceros.Razonsocial, 
                      dbo.Terceros.Direccion, dbo.Documentos.MontoGravable, dbo.Documentos.MontoExento, dbo.Documentos.MontoIva, dbo.Documentos.MontoTotal
FROM         dbo.Documentos INNER JOIN
                      dbo.Terceros ON dbo.Documentos.IdTercero = dbo.Terceros.IdTercero