from graphlib import CycleError
from textwrap import fill
from turtle import fillcolor
import pygame
from pygame.locals import *
from Text import Text
from TextBox import TextBox
from colors import *
from Window import Window
from Surface import Surface
from Button import Button
from Style import Style


class Wordle:
	"""A class to play the game Wordle
	"""

	size = (400, 550)
	"""The size of the game window"""
	width = size[0]
	"""The width of the game window"""
	height = size[1]
	"""The height of the game window"""


	caption = "WORDLE"
	"""The caption for the game window"""
	backgroundColor = (23, 23, 23)
	"""The background color for the game window"""
	clock = pygame.time.Clock()
	"""The clock for the game to keep track of the time between each frame"""
	font = "HelveticaNeueBold.ttf"
	"""The font for the game"""

	def __init__(self):
		"""Initializes the Game
		"""
		# Initializes pygame and creates the window
		pygame.init()
		self.window = Window(self.size, self.caption, self.backgroundColor)

	def Start(self):
		"""Shows the start screen for the game
		"""
		# create the startScreen		
		startScreen = Surface((0, 0), self.size, self.backgroundColor)

		# Create the title for the startScreen
		title = Text((self.width / 2, 100), self.font, 75, "WORDLE", WHITE)

		# Create the Log in button for the startScreen
		logInButton = Button((self.width/ 4 - 75 + 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
							Style(Text((75, 25), self.font, 25, "LOG IN", DENIM),
							fillColor=self.backgroundColor, borderColor=DENIM, borderRadius=5),

                    		hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
							fillColor=DENIM, borderColor=DENIM, borderRadius=5))

		# Create the sign up button for the startScreen
		signUpButton = Button((self.width * 3 / 4 - 75 - 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
								Style(Text((75, 25), self.font, 24, "SIGN UP", ORCHID),
								fillColor=self.backgroundColor, borderColor=ORCHID, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 25, "SIGN UP", WHITE),
								fillColor=ORCHID, borderColor=ORCHID, borderRadius=5))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (logInButton.mouseIsHovering(mousePos)):
						self.LogIn()
						return

					# if the mouse clicked the sign up button
					elif (signUpButton.mouseIsHovering(mousePos)):
						self.SignUp()
						return

			# render the background rectangles
			pygame.draw.rect(startScreen.display, TURQUOISE, (-20, -20, 300, 400), 2, 10)
			pygame.draw.rect(startScreen.display, CYBERGRAPE, (205, 200, 500, 500), 2, 10)
			pygame.draw.rect(startScreen.display, LIGHTGREEN, (-20, -20, 112, 600), 2, 10)


			# render the title
			startScreen.display.blit(title.display, title.rect)

			# render the buttons

			# render the log in button
			logInButton.render(mousePos)
			startScreen.display.blit(logInButton.surface.display, logInButton.pos)

			# render the sign up button
			signUpButton.render(mousePos)
			startScreen.display.blit(signUpButton.surface.display, signUpButton.pos)

			# render the screen
			self.window.display.blit(startScreen.display, startScreen.pos)

			# update
			pygame.display.update()

	def LogIn(self):
		""" Shows the log in screen for the game
		"""
		# create the log in screen
		logInScreen = Surface((0, 0), self.size, self.backgroundColor)
		
		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 50), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button((header.width / 2 - 75, 2), Surface((0, 0), (150, 50), CYBERGRAPE),
								Style(Text((150 / 2, 50 / 2), self.font, 25, "WORDLE", WHITE),
								fillColor=CYBERGRAPE))



		# Create the title for the screen
		title = Text((self.width / 2, 125), self.font, 75, "LOG IN", WHITE)

		# Create the Username label for the screen
		usernameLbl = Text((self.width / 4 + 20, 225), self.font, 30, "USERNAME", WHITE)

		# Create the username text box for the screen
		usernameTxt = TextBox((self.width / 4 - 30, 250), Surface((0, 0), (300, 35), self.backgroundColor),
								Style(Text((10, 35 / 2), self.font, 20, "", WHITE),
                    			borderColor=TURQUOISE, borderRadius=5),

                   		 		selectedStyle=Style(borderColor=TURQUOISE, borderWidth=4, borderRadius=5))

		# Create the Password label for the screen
		passwordLbl = Text((self.width / 4 + 20, 325), self.font, 30, "PASSWORD", WHITE)

		# Create the password text box for the screen
		passwordTxt = TextBox((self.width / 4 - 30, 350), Surface((0, 0), (300, 35), self.backgroundColor),
                        		Style(Text((10, 35 / 2), self.font, 20, "", WHITE),
								borderColor=TURQUOISE, borderRadius=5),

                        		selectedStyle=Style(borderColor=TURQUOISE, borderWidth=4, borderRadius=5))

		# Create the login button for the screen
		logInButton = Button((self.width / 2 + 20, 415), Surface((0, 0), (150, 50), self.backgroundColor),
                       			Style(Text((75, 25), self.font, 25, "LOG IN", DENIM),
                                borderColor=DENIM, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
                         fillColor=DENIM, borderRadius=5))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return
					elif (logInButton.mouseIsHovering(mousePos)):
						self.Start()
						return

					# check if the mouse hit the username text box
					usernameTxt.mouseClick(mousePos)

					# check if th mouse hit the password text box
					passwordTxt.mouseClick(mousePos)


				# Checks for the KEYDOWN event
				if event.type == pygame.KEYDOWN:
					# if the return key is pressed
					if event.key == pygame.K_RETURN:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# attempt to log in
						elif(passwordTxt.isSelected):
							passwordTxt.isSelected = False
							self.Start()
							return

					# if the tab key is pressed
					elif event.key == pygame.K_TAB:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# jump to the username text box
						else:
							passwordTxt.isSelected = False
							usernameTxt.isSelected = True

					# if the key is the backspace
					elif event.key == pygame.K_BACKSPACE:
						# if username text box is selected -> backspace
						if (usernameTxt.isSelected):
							usernameTxt.backSpace()
						# if password text box is selected -> backspace
						elif (passwordTxt.isSelected):
							passwordTxt.backSpace()
					else:
						# if the username text box is selevted -> insert the character
						if (usernameTxt.isSelected):
							usernameTxt.insert(event.unicode)
						# if the password text box is selevted -> insert the character
						elif (passwordTxt.isSelected):
							passwordTxt.insert(event.unicode)
				


			# create the rectangles
			pygame.draw.rect(logInScreen.display, LIGHTGREEN, (-33, 69, 320, 700), 2, 10)
			pygame.draw.rect(logInScreen.display, ORCHID, (130, 200, 600, 600), 2, 10)
			pygame.draw.rect(logInScreen.display, CYBERGRAPE, (-33, 500, 600, 300), 2, 10)

			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.pos)
			logInScreen.display.blit(header.display, header.pos)

			# render the title
			logInScreen.display.blit(title.display, title.rect)

			# render the username label
			logInScreen.display.blit(usernameLbl.display, usernameLbl.rect)

			# render the username Text box
			usernameTxt.render()
			logInScreen.display.blit(usernameTxt.surface.display, usernameTxt.pos)

			# render the password label
			logInScreen.display.blit(passwordLbl.display, passwordLbl.rect)
			
			# render the password Text Box
			passwordTxt.render()
			logInScreen.display.blit(passwordTxt.surface.display, passwordTxt.pos)

			# render the login button
			logInButton.render(mousePos)
			logInScreen.display.blit(logInButton.surface.display, logInButton.pos)

			# render the screen
			self.window.display.blit(logInScreen.display, logInScreen.pos)

			# update
			pygame.display.update()
		
	def SignUp(self):
		""" Shows the sign up screen for the game
		"""
		# create the log in screen
		signUpScreen = Surface((0, 0), self.size, self.backgroundColor)
		
		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 50), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button((header.width / 2 - 75, 2), Surface((0, 0), (150, 50), CYBERGRAPE),
								Style(Text((150 / 2, 50 / 2), self.font, 25, "WORDLE", WHITE),
								fillColor=CYBERGRAPE))



		# Create the title for the screen
		title = Text((self.width / 2, 100), self.font, 75, "SIGN UP", WHITE)



		# Create the Username label for the screen
		usernameLbl = Text((self.width / 4 + 20, 200), self.font, 25, "USERNAME", WHITE)

		# Create the username text box for the screen
		usernameTxt = TextBox((self.width / 4 - 30, 225), Surface((0, 0), (300, 30), self.backgroundColor),
								Style(Text((10, 20 / 2), self.font, 15, "", WHITE),
                    			borderColor=LIGHTGREEN, borderRadius=5),

                   		 		selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))




		# Create the Password label for the screen
		passwordLbl = Text((self.width / 4 + 20, 300), self.font, 25, "PASSWORD", WHITE)

		# Create the password text box for the screen
		passwordTxt = TextBox((self.width / 4 - 30, 325), Surface((0, 0), (300, 30), self.backgroundColor),
                        		Style(Text((10, 20 / 2), self.font, 15, "", WHITE),
                                borderColor=LIGHTGREEN, borderRadius=5),

                        		selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))

		# Create the verify Password label for the screen
		verifyPasswordLbl = Text((self.width / 4 + 70, 400), self.font, 25, "VERIFY PASSWORD", WHITE)

		# Create the verify password text box for the screen
		verifyPasswordTxt = TextBox((self.width / 4 - 30, 425), Surface((0, 0), (300, 30), self.backgroundColor),
                        		Style(Text((10, 20 / 2), self.font, 15, "", WHITE),
                                borderColor=LIGHTGREEN, borderRadius=5),

                       			selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))

		# Create the login button for the screen
		logInButton = Button((self.width / 2 + 20, 440), Surface((0, 0), (150, 50), self.backgroundColor),
                       			Style(Text((75, 25), self.font, 25, "LOG IN", ORCHID),
                                borderColor=ORCHID, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
                         		fillColor=ORCHID, borderRadius=5))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return
					elif (logInButton.mouseIsHovering(mousePos)):
						self.Start()
						return

					# check if the mouse hit the username text box
					usernameTxt.mouseClick(mousePos)

					# check if th mouse hit the password text box
					passwordTxt.mouseClick(mousePos)


				# Checks for the KEYDOWN event
				if event.type == pygame.KEYDOWN:
					# if the return key is pressed
					if event.key == pygame.K_RETURN:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# attempt to log in
						elif(passwordTxt.isSelected):
							passwordTxt.isSelected = False
							self.Start()
							return

					# if the tab key is pressed
					elif event.key == pygame.K_TAB:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# jump to the username text box
						else:
							passwordTxt.isSelected = False
							usernameTxt.isSelected = True

					# if the key is the backspace
					elif event.key == pygame.K_BACKSPACE:
						# if username text box is selected -> backspace
						if (usernameTxt.isSelected):
							usernameTxt.backSpace()
						# if password text box is selected -> backspace
						elif (passwordTxt.isSelected):
							passwordTxt.backSpace()
					else:
						# if the username text box is selevted -> insert the character
						if (usernameTxt.isSelected):
							usernameTxt.insert(event.unicode)
						# if the password text box is selevted -> insert the character
						elif (passwordTxt.isSelected):
							passwordTxt.insert(event.unicode)
				


			# create the rectangles
			pygame.draw.rect(signUpScreen.display, TURQUOISE, (-33, 69, 320, 700), 2, 10)
			pygame.draw.rect(signUpScreen.display, DENIM, (130, 200, 600, 600), 2, 10)
			pygame.draw.rect(signUpScreen.display, CYBERGRAPE, (-33, 500, 600, 300), 2, 10)

			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.pos)
			signUpScreen.display.blit(header.display, header.pos)

			# render the title
			signUpScreen.display.blit(title.display, title.rect)

			# render the username label
			signUpScreen.display.blit(usernameLbl.display, usernameLbl.rect)

			# render the username Text box
			usernameTxt.render()
			signUpScreen.display.blit(usernameTxt.surface.display, usernameTxt.pos)

			# render the password label
			signUpScreen.display.blit(passwordLbl.display, passwordLbl.rect)

			# render the password Text Box
			passwordTxt.render()
			signUpScreen.display.blit(passwordTxt.surface.display, passwordTxt.pos)

			# render the password label
			signUpScreen.display.blit(verifyPasswordLbl.display, verifyPasswordLbl.rect)

			# render the password Text Box
			verifyPasswordTxt.render()
			signUpScreen.display.blit(verifyPasswordTxt.surface.display, verifyPasswordTxt.pos)

			# render the login button
			logInButton.render(mousePos)
			signUpScreen.display.blit(logInButton.surface.display, logInButton.pos)

			# render the screen
			self.window.display.blit(signUpScreen.display, signUpScreen.pos)

			# update
			pygame.display.update()



wordle = Wordle()
wordle.Start()