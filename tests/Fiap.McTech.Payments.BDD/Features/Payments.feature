#coding:utf-8

Feature: Gerar QR-Code para pagamento
	Given um usu�rio
	deve ser capaz de gerar um c�digo QR-Code
	para efetuar o pagamento do mesmo

	Scenario: Gerar um QR-Code para pagamento
		Given que eu tenho os dados v�lidos para gerar um QR-Code para pagamento
		When eu solicitar para a rota gerar o QR-Code
		Then o status de resposta deve ser 200 OK
		And o pagamento deve ser registrado no sistema