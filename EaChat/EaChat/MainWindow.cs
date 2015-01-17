﻿//
//  MainWindow.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Xwt;
using Xwt.Formats;

namespace EaChat
{
	public partial class MainWindow
	{
		ListStore chatStore;
		DataField<int> publishersCol;
		DataField<int> subscribersCol;
		DataField<string> chatNameCol;

		ParticipantController controller;
		string chatText;

		public MainWindow()
		{
			CreateComponents();

			publishersCol  = new DataField<int>();
			subscribersCol = new DataField<int>();
			chatNameCol    = new DataField<string>();

			chatStore = new ListStore(publishersCol, subscribersCol, chatNameCol);
			chatList.DataSource = chatStore;
			chatList.Columns.Add("Publishers", publishersCol);
			chatList.Columns.Add("Subscribers", subscribersCol);
			chatList.Columns.Add("Name", chatNameCol);

			string userName = AskUserName();
			controller = new ParticipantController(userName, this);

			chatTextEntry.KeyPressed += HandleKeySendPressed;
			chatSendBtn.Clicked += HandleSend;
			CloseRequested += HandleCloseRequested;
		}

		public void ShowMessage(ChatMessage instance)
		{
			chatText += string.Format("[{0}] {1}: {2}\n", 
				instance.UserName, instance.Date, instance.Message);
			chatView.Markdown += chatText;
		}

		public void UpdateChatInfo(string chatName, int numPub, int numSub)
		{
			int row = FindRow(chatName);
			if (row == -1)
				AddChatInfo(chatName, numPub, numSub);
			else
				SetChatInfo(row, numPub, numSub);
		}

		int FindRow(string chatName)
		{
			for (int i = 0; i < chatStore.RowCount; i++)
				if (chatStore.GetValue(i, chatNameCol) == chatName)
					return i;

			return -1;
		}

		void AddChatInfo(string chatName, int numPub, int numSub)
		{
			int row = chatStore.AddRow();
			chatStore.SetValues(row,
				publishersCol,  numPub,
				subscribersCol, numSub,
				chatNameCol,    chatName
			);
		}

		void SetChatInfo(int row, int numPub, int numSub)
		{
			chatStore.SetValue(row, publishersCol, numPub);
			chatStore.SetValue(row, subscribersCol, numSub);
		}

		void HandleKeySendPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
				Send();
		}

		void HandleSend(object sender, EventArgs e)
		{
			Send();
		}

		void Send()
		{
			controller.Send(chatTextEntry.Text, null);
			chatTextEntry.Text = string.Empty;
		}

		void HandleCloseRequested(object sender, CloseRequestedEventArgs args)
		{
			Application.Exit();
		}
	}
}

